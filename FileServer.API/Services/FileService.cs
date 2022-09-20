﻿using FileServer.API.Models;
using FileServer.API.Models.Repository;
using Microsoft.AspNetCore.Http;
using ModelLibrary;
using System.Net;
using System.Net.Security;

namespace FileServer.API.Services
{
    public class FileService : IFileService
    {
        private readonly IConfiguration _configuration;
        private readonly IFileRepository _fileRepository;

        public FileService(IConfiguration configuration, IFileRepository fileRepository)
        {
            _configuration = configuration;
            _fileRepository = fileRepository;
        }

        public async Task<Result<ImageFile>> UploadFileAsync(IFormFile file)
        {
            try
            {
                string basePath = Path.Combine(Directory.GetCurrentDirectory() + "/Files/");
                string fileName = Path.GetFileName(file.FileName);
                string newFileName = string.Concat($"ceg-img-{DateTime.Now.Ticks.ToString()[12..]}-", fileName);
                string filePath = string.Concat($"{basePath}", newFileName);

                string url = $"{_configuration["Urls:ProdBaseUrl"]}/uploads/{newFileName}";
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(_configuration["FtpServer:Url"] + newFileName);
                request.Method = WebRequestMethods.Ftp.UploadFile;
                request.EnableSsl = true;
                
                ServicePointManager.Expect100Continue = false;

                // This example assumes the FTP site uses anonymous logon.
                request.Credentials = new NetworkCredential(_configuration["FtpServer:UserName"], _configuration["FtpServer:Password"]);

                // Copy the contents of the file to the request stream.
                using (FileStream fileStream = File.Open(filePath, FileMode.Open, FileAccess.Read))
                {
                    using Stream requestStream = request.GetRequestStream();
                    await fileStream.CopyToAsync(requestStream);
                }

                //using (var client = new WebClient())
                //{
                //    client.Credentials = new NetworkCredential(_configuration["FtpServer:UserName"], _configuration["FtpServer:Password"]);
                //    client.UploadFile(_configuration["FtpServer:Url"] + newFileName, WebRequestMethods.Ftp.UploadFile, filePath);
                //}

                var result = await _fileRepository.AddAsync(new ImageFile
                {
                    FileName = newFileName,
                    Url = url,
                    Path = filePath
                });

                return result;
            }
            catch (Exception ex)
            {
                return new Result<ImageFile>(false, new List<string> { ex.ToString() });
            }
        }

        public async Task<Result<bool>> DeleteFileAsync(string fileName)
        {
            var recordIsDeleted = (await _fileRepository.DeleteAsync(fileName)).Data;
            if (!recordIsDeleted) return new Result<bool>(false);

            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(_configuration["FtpServer:Url"] + fileName);
            request.Method = WebRequestMethods.Ftp.DeleteFile;
            request.Credentials = new NetworkCredential(_configuration["FtpServer:UserName"], _configuration["FtpServer:Password"]);

            using FtpWebResponse response = (FtpWebResponse)request.GetResponse();
            if (response.StatusCode != FtpStatusCode.FileActionOK) return await Task.FromResult(new Result<bool>(false));

            return await Task.FromResult(new Result<bool>(true));
        }
    }
}