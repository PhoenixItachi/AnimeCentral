using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

public class VideoUtils
{
    public static List<string> VideoAcceptedFormats = new List<string>() { "video/mp4", "video/webm", "video/ogg" };

    public static string videosFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "Videos");
    public static async Task<string> SaveVideo(IFormFile video)
    {
        try
        {
            using (var fileStream = new FileStream(Path.Combine(videosFolderPath, video.FileName), FileMode.Create))
            {
                await video.CopyToAsync(fileStream);
            }

            return video.FileName;
        }
        catch (Exception e)
        {
            return String.Empty;
        }
    }

    public class VideoResult : ActionResult
    {
        public readonly string _videoPath;
        public VideoResult(string fileName)
        {
            _videoPath = Path.Combine(videosFolderPath, fileName);
        }
        /// <summary> 
        /// The below method will respond with the Video file 
        /// </summary> 
        /// <param name="context"></param> 
        public override void ExecuteResult(ActionContext context)
        {
            var video = new FileInfo(_videoPath);
            if (!video.Exists)
            {
                context.HttpContext.Response.StatusCode = StatusCodes.Status404NotFound;
                return;
            }
                
            long fSize = video.Length;
            long startbyte = 0;
            long endbyte = fSize - 1;
            int statusCode = 200;
            if (context.HttpContext.Request.Headers.ContainsKey("Range"))
            {
                //Get the actual byte range from the range header string, and set the starting byte.
                string[] range = context.HttpContext.Request.Headers["Range"].ToString().Split(new char[] { '=', '-' });
                startbyte = Convert.ToInt64(range[1]);
                if (range.Length > 2 && range[2] != "") endbyte = Convert.ToInt64(range[2]);
                //If the start byte is not equal to zero, that means the user is requesting partial content.
                if (startbyte != 0 || endbyte != fSize - 1 || range.Length > 2 && range[2] == "")
                { statusCode = 206; }//Set the status code of the response to 206 (Partial Content) and add a content range header.                                    
            }
            long desSize = endbyte - startbyte + 1;

            if (video.Exists)
            {
                using (var videoStream = video.OpenRead())
                {
                    //The header information 
                    videoStream.Seek(startbyte, SeekOrigin.Begin);
                    var response = context.HttpContext.Response;
                    response.StatusCode = statusCode;
                    response.ContentType = context.HttpContext.Response.ContentType;
                    response.Headers.Add("Content-Accept", response.ContentType);
                    response.Headers.Add("Content-Length", desSize.ToString());
                    response.Headers.Add("Content-Range", string.Format("bytes {0}-{1}/{2}", startbyte, endbyte, fSize));
                    videoStream.CopyTo(context.HttpContext.Response.Body, (int)desSize);
                }
            }
        }
    }
}