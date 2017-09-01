
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

public class VideoUtils
{
    public readonly string _filename;

    public VideoUtils(string filename, string ext)
    {
        string startupPath = Directory.GetCurrentDirectory();
        var videoPath = Path.Combine(startupPath, "Videos", $"{filename}.{ext}");
        _filename = videoPath;
    }

    public async Task WriteToStream(HttpContext context)
    {
        try
        {
            var buffer = new byte[65536];
            using (var video = File.Open(_filename, FileMode.Open, FileAccess.Read))
            {
                var length = (int)video.Length;
                var bytesRead = 1;

                while (length > 0 && bytesRead > 0)
                {
                    bytesRead = await video.ReadAsync(buffer, 0, Math.Min(length, buffer.Length));
                    await context.Response.Body.WriteAsync(buffer, 0, buffer.Length);
                    length -= bytesRead;
                }
            }
        }
        catch (Exception ex)
        {
            return;
        }
        finally
        {
        }
    }

    public class VideoResult : ActionResult
    {
        public readonly string _filename;
        public VideoResult(string filename, string ext)
        {
            string startupPath = Directory.GetCurrentDirectory();
            var videoPath = Path.Combine(startupPath, "Videos", $"{filename}.{ext}");
            _filename = videoPath;
        }
        /// <summary> 
        /// The below method will respond with the Video file 
        /// </summary> 
        /// <param name="context"></param> 
        public override async void ExecuteResult(ActionContext context)
        {
            var video = new FileInfo(_filename);

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
            //The header information 
            var response = context.HttpContext.Response;
            //Check the file exist,  it will be written into the response 
            if (video.Exists)
            {
                var stream = video.OpenRead();
                var bytesinfile = new byte[stream.Length];
                stream.Read(bytesinfile, 0, (int)video.Length);

                response.StatusCode = statusCode;
                response.ContentType = "video/mp4";
                response.Headers.Add("Content-Accept", response.ContentType);
                response.Headers.Add("Content-Length", desSize.ToString());
                response.Headers.Add("Content-Range", string.Format("bytes {0}-{1}/{2}", startbyte, endbyte, fSize));

                await context.HttpContext.Response.Body.WriteAsync(bytesinfile, (int)startbyte, (int)desSize);
            }
        }
    }
}