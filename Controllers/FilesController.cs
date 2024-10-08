﻿using ST10028058_CLDV6212_POE.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

public class FilesController : Controller
{
    private readonly AzureFileShareService _fileShareService;

    public FilesController(AzureFileShareService fileShareService)
    {
        _fileShareService = fileShareService;
    }

    public async Task<IActionResult> Index()
    {
        List<FileModel> files;
        try
        {
            files = await _fileShareService.ListFilesAsync("uploads");
        }
        catch (Exception ex)
        {
            ViewBag.Message = $"Failed to load files: {ex.Message}";
            files = new List<FileModel>();
        }

        return View(files);
    }

    [HttpPost]
    public async Task<IActionResult> Upload(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            ModelState.AddModelError("file", "Please select a file to upload.");
            return await Index();  
        }

        try
        {
            using (var stream = file.OpenReadStream())
            {
                string directoryName = "uploads";  
                string fileName = file.FileName;   

                await _fileShareService.UploadFileAsync(directoryName, fileName, stream);
            }

            TempData["Message"] = $"File '{file.FileName}' uploaded successfully!";
        }
        catch (Exception ex)
        {
            TempData["Message"] = $"File upload failed: {ex.Message}";
        }

        return RedirectToAction("Index"); 
    }

    // Handle file download
    [HttpGet]
    public async Task<IActionResult> DownloadFile(string fileName)
    {
        if (string.IsNullOrEmpty(fileName))
        {
            return BadRequest("File name cannot be null or empty.");
        }

        try
        {
            var fileStream = await _fileShareService.DownloadFileAsync("uploads", fileName);

            if (fileStream == null)
            {
                return NotFound($"File '{fileName}' not found.");
            }

            return File(fileStream, "application/octet-stream", fileName);
        }
        catch (Exception ex)
        {
            return BadRequest($"Error downloading file: {ex.Message}");
        }
    }
}

//# Assistance provided by ChatGPT
//# Code and support generated with the help of OpenAI's ChatGPT.
// code attribution
// W3schools
// https://www.w3schools.com/cs/index.php

// code attribution
//Microsoft
//https://learn.microsoft.com/en-us/aspnet/mvc/overview/getting-started/introduction/getting-started

// code attribution
//Microsoft
//https://learn.microsoft.com/en-us/azure/storage/blobs/storage-blob-dotnet-get-started?tabs=azure-ad

// code attribution
//Bootswatch
//https://bootswatch.com/