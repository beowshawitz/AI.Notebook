﻿using System.ComponentModel.DataAnnotations;

namespace AI.Notebook.Web.Models;

public class SettingsResultTypeModel
{
	[Required]
	public int Id { get; set; } = 0;

	[Required]
	[Display(Name = "Result Type Name")]
	[MinLength(2, ErrorMessage = "The {0} must be at least {1} characters.")]
	[MaxLength(150)]
	public string Name { get; set; } = string.Empty;

	[Display(Name = "Result Type Description")]
	[MaxLength(250)]
	public string? Description { get; set; } = string.Empty;

	public bool IsValid()
	{
		bool valid = true;
		if (string.IsNullOrEmpty(Name))
			valid = false;
		return valid;
	}
}
