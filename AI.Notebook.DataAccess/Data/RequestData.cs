﻿using AI.Notebook.DataAccess.DBAccess;
using AI.Notebook.Common.Models;
using Dapper;
using System.Data;

namespace AI.Notebook.DataAccess.Data;
public class RequestData : IRequestData
{
	private readonly ISqlDataAccess _dataAccess;

	public RequestData(ISqlDataAccess dataAccess)
	{
		_dataAccess = dataAccess;
	}

	public async Task<IEnumerable<RequestModel>> GetAllAsync()
	{
		var requests = await _dataAccess.LoadDataAsync<RequestModel, dynamic>("dbo.spRequests_GetAll", new { });
		if (requests != null)
		{
			AIResourceData aiResourceDataItems = new AIResourceData(_dataAccess);
			IEnumerable<AIResourceModel> aiResources = await aiResourceDataItems.GetAllAsync();
			foreach (var request in requests)
			{
				request.AIResource = aiResources.First(x => x.Id == request.ResourceId);
			}
			return requests;
		}

		return Enumerable.Empty<RequestModel>();
	}

	public async Task<PageResultModel<RequestModel>> GetPagedAsync(PageSubmissionModel pageRequest)
	{
		PageResultModel<RequestModel> results = new PageResultModel<RequestModel>(pageRequest.PageSize, pageRequest.Start);
		var p = new DynamicParameters();
		p.Add(name: "@SortBy", pageRequest.SortBy);
		p.Add(name: "@SortOrder", pageRequest.SortDirection);
		p.Add(name: "@PageSize", pageRequest.PageSize);		
		p.Add(name: "@Start", pageRequest.Start);
		p.Add(name: "@PageSize", pageRequest.PageSize);
		if(!string.IsNullOrEmpty(pageRequest.Filter))
		{
			p.Add(name: "@Search", pageRequest.Filter);
		}
		if (pageRequest.BeginDate.HasValue)
		{
			p.Add(name: "@Begin", pageRequest.BeginDate.Value.Date);
		}
		if (pageRequest.EndDate.HasValue)
		{
			p.Add(name: "@End", pageRequest.EndDate.Value.Date);
		}
		p.Add(name: "@Total", value: 0, dbType: DbType.Int32, direction: ParameterDirection.Output);
		var requests = await _dataAccess.LoadDataAsync<RequestModel, dynamic>("dbo.spRequests_GetPaged", p);
		if(requests != null)
		{
			results.ItemCount = p.Get<int>("@Total");
			AIResourceData aiResourceDataItems = new AIResourceData(_dataAccess);
			IEnumerable<AIResourceModel> aiResources = await aiResourceDataItems.GetAllAsync();
			foreach(var request in requests)
			{
				request.AIResource = aiResources.First(x=>x.Id == request.ResourceId);
			}
			results.Collection = requests;
			return results;
		}
		
		return new PageResultModel<RequestModel>();
	}

	public async Task<RequestModel?> GetAsync(int id)
	{
		var results = await _dataAccess.LoadDataAsync<RequestModel, dynamic>("dbo.spRequests_Get", new { Id = id });
		if (results != null)
		{
			var request = results.FirstOrDefault();
			if(request != null) 
			{
				AIResourceData data = new AIResourceData(_dataAccess);
				request.AIResource = await data.GetAsync(request.ResourceId);
				return request;
			}
		}
		return null;
	}

	public async Task<RequestTranslatorModel?> GetTranslatorAsync(int requestId)
	{
		var requestModel = await GetAsync(requestId);
		if (requestModel != null && requestModel.Id > 0)
		{
			var results = await _dataAccess.LoadDataAsync<RequestTranslatorModel, dynamic>("dbo.spRequestsTranslator_Get", new { RequestId = requestId });
			if (results != null)
			{
				var request = results.FirstOrDefault();
				if (request != null)
				{
					AIResourceData data = new AIResourceData(_dataAccess);
					request.AIResource = await data.GetAsync(request.ResourceId);
					request.CopyBaseData(requestModel);
					return request;
				}
			}
		}
		return null;
	}

	public async Task<RequestSpeechModel?> GetSpeechAsync(int requestId)
	{
		var requestModel = await GetAsync(requestId);
		if (requestModel != null && requestModel.Id > 0)
		{
			var results = await _dataAccess.LoadDataAsync<RequestSpeechModel, dynamic>("dbo.spRequestsSpeech_Get", new { RequestId = requestId });
			if (results != null)
			{
				var request = results.FirstOrDefault();
				if (request != null)
				{
					AIResourceData data = new AIResourceData(_dataAccess);
					request.AIResource = await data.GetAsync(request.ResourceId);
					request.CopyBaseData(requestModel);
					return request;
				}
			}
		}
		return null;
	}

	public async Task<RequestVisionModel?> GetVisionAsync(int requestId)
	{
		var requestModel = await GetAsync(requestId);
		if (requestModel != null && requestModel.Id > 0)
		{
			var results = await _dataAccess.LoadDataAsync<RequestVisionModel, dynamic>("dbo.spRequestsVision_Get", new { RequestId = requestId });
			if (results != null)
			{
				var request = results.FirstOrDefault();
				if (request != null)
				{
					AIResourceData data = new AIResourceData(_dataAccess);
					request.AIResource = await data.GetAsync(request.ResourceId);
					request.CopyBaseData(requestModel);
					return request;
				}
			}
		}
		return null;
	}

	public async Task<RequestLanguageModel?> GetLanguageAsync(int requestId)
	{
		var requestModel = await GetAsync(requestId);
		if (requestModel != null && requestModel.Id > 0)
		{
			var results = await _dataAccess.LoadDataAsync<RequestLanguageModel, dynamic>("dbo.spRequestsLanguage_Get", new { RequestId = requestId });
			if (results != null)
			{
				var request = results.FirstOrDefault();
				if (request != null)
				{
					AIResourceData data = new AIResourceData(_dataAccess);
					request.AIResource = await data.GetAsync(request.ResourceId);
					request.CopyBaseData(requestModel);
					return request;
				}
			}
		}
		return null;
	}

	public async Task<int> InsertAsync(RequestModel item)
	{
		var p = new DynamicParameters();
		p.Add(name: "@ResourceId", item.ResourceId);
		p.Add(name: "@Name", item.Name);
		p.Add(name: "@Id", value:0, dbType: DbType.Int32, direction: ParameterDirection.Output);
		p.Add(name: "@Output", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

		await _dataAccess.SaveDataAsync("dbo.spRequests_Insert", p);
		var newId = p.Get<int?>("@Id");
		return newId.HasValue ? newId.Value : 0;
	}

	public async Task<int> InsertTranslatorAsync(RequestTranslatorModel item)
	{
		var p = new DynamicParameters();
		p.Add(name: "@RequestId", item.RequestId);
		p.Add(name: "@Id", value: 0, dbType: DbType.Int32, direction: ParameterDirection.Output);
		p.Add(name: "@Output", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

		await _dataAccess.SaveDataAsync("dbo.spRequestsTranslator_Insert", p);
		var newId = p.Get<int?>("@Id");
		return newId.HasValue ? newId.Value : 0;
	}

	public async Task<int> InsertSpeechAsync(RequestSpeechModel item)
	{
		var p = new DynamicParameters();
		p.Add(name: "@RequestId", item.RequestId);
		p.Add(name: "@Id", value: 0, dbType: DbType.Int32, direction: ParameterDirection.Output);
		p.Add(name: "@Output", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

		await _dataAccess.SaveDataAsync("dbo.spRequestsSpeech_Insert", p);
		var newId = p.Get<int?>("@Id");
		return newId.HasValue ? newId.Value : 0;
	}

	public async Task<int> InsertVisionAsync(RequestVisionModel item)
	{
		var p = new DynamicParameters();
		p.Add(name: "@RequestId", item.RequestId);
		p.Add(name: "@Id", value: 0, dbType: DbType.Int32, direction: ParameterDirection.Output);
		p.Add(name: "@Output", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

		await _dataAccess.SaveDataAsync("dbo.spRequestsVision_Insert", p);
		var newId = p.Get<int?>("@Id");
		return newId.HasValue ? newId.Value : 0;
	}

	public async Task<int> InsertLanguageAsync(RequestLanguageModel item)
	{
		var p = new DynamicParameters();
		p.Add(name: "@RequestId", item.RequestId);
		p.Add(name: "@Id", value: 0, dbType: DbType.Int32, direction: ParameterDirection.Output);
		p.Add(name: "@Output", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

		await _dataAccess.SaveDataAsync("dbo.spRequestsLanguage_Insert", p);
		var newId = p.Get<int?>("@Id");
		return newId.HasValue ? newId.Value : 0;
	}

	public int Update(RequestModel item)
	{
		return _dataAccess.SaveData<dynamic>("dbo.spRequests_Update", new { item.Id, item.ResourceId, item.Name});
	}

	public int UpdateTranslator(RequestTranslatorModel item)
	{
		dynamic parameters = new { item.Name, item.RequestId, item.SourceLangCode, item.TargetLangCode, item.Input, item.Translate, item.Transliterate, item.OutputAsAudio, item.Ssml, item.SsmlUrl, item.VoiceName };
		return _dataAccess.SaveData<dynamic>("dbo.spRequestsTranslator_Update", parameters);
	}

	public int UpdateSpeech(RequestSpeechModel item)
	{
		dynamic parameters = new { item.Name, item.RequestId, item.SourceLangCode, item.TargetLangCode, item.AudioData, item.AudioUrl, item.Ssml, item.SsmlUrl, item.OutputAsAudio, item.VoiceName, item.Translate, item.Transcribe };
		return _dataAccess.SaveData<dynamic>("dbo.spRequestsSpeech_Update", parameters);
	}

	public int UpdateVision(RequestVisionModel item)
	{
		dynamic parameters = new { item.Name, item.RequestId, item.ImageData, item.ImageUrl, item.GenderNeutralCaption, item.Caption, item.DenseCaptions, item.Tags, item.ObjectDetection, item.People, item.SmartCrop, item.Ocr };
		return _dataAccess.SaveData<dynamic>("dbo.spRequestsVision_Update", parameters);
	}

	public int UpdateLanguage(RequestLanguageModel item)
	{
		dynamic parameters = new { item.Name, item.RequestId, item.SourceLangCode, item.TargetLangCode, item.Input, item.Language, item.Sentiment, item.KeyPhrases, item.Entities, item.PiiEntites, item.LinkedEntities, item.NamedEntityRecognition, item.Summary, item.AbstractiveSummary };
		return _dataAccess.SaveData<dynamic>("dbo.spRequestsLanguage_Update", parameters);
	}

	public async Task<bool> DeleteAsync(int id)
	{
		var p = new DynamicParameters();
		p.Add(name: "@Id", id);
		p.Add(name: "@Output", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

		await _dataAccess.SaveDataAsync("dbo.spRequests_Delete", p);
		var completed = p.Get<int?>("@Output");
		if(completed.HasValue && completed.Value.Equals(1))
		{
			return true;
		}
		else
		{
			return false;
		}
	}
}
