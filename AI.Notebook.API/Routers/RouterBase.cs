﻿namespace AI.Notebook.API.Routers;

public class RouterBase
{
	public string UrlFragment;
	protected ILogger Logger;

	public virtual void AddRoutes(WebApplication app)
	{
	}
}
