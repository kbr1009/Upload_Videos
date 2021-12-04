using System;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Net;
using System.IO;

namespace FileUploader
{

  [DataContract]
  public class JsonData
  {
    [DataMember (Name = "PUT_URL")]
    public string PUT_URL { get; set; }

    [DataMember (Name = "UUID")]
    public string UUID { get; set; }
  }

  public class GetUrl
  {
    const string s3_url = "<aws s3の署名付きurl>";

    public string S3_url()
    {
      HttpWebRequest req = (HttpWebRequest)WebRequest.Create(s3_url);
      HttpWebResponse res = (HttpWebResponse)req.GetResponse();

      JsonData info;
      using (res)
      {
        using (var resStream = res.GetResponseStream())
        {
          var serializer = new DataContractJsonSerializer(typeof(JsonData));
          info = (JsonData)serializer.ReadObject(resStream);
        }
      }
      return info.PUT_URL;
    }
  }

  class Uploder
  {
    public static void Main()
    {
      GetUrl get_s3_url = new GetUrl();

      Console.WriteLine("保存したい動画を入力してください");
      string video = Console.ReadLine();
      string videopath = "./file/" + video;

      using (FileStream stream = new FileStream(videopath, FileMode.Open, FileAccess.Read)) 
      {

        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(get_s3_url.S3_url());
        request.Method = "PUT";
        request.ContentType = "video/mp4";
        request.ContentLength = stream.Length;
        request.AllowWriteStreamBuffering = false;

        request.Timeout = 360 * 60 * 1000;
        request.ReadWriteTimeout = 360 * 60 * 1000;

        try {
          using (Stream requestStream = request.GetRequestStream()) {
            stream.CopyTo(requestStream);
            Console.WriteLine("OK!");
          }
        } catch (Exception) {
          Console.WriteLine("ファイルの読み込みエラー");
        }

        try {
          request.GetResponse();
          Console.WriteLine("OK!");
        } catch (Exception) {
          Console.WriteLine("アップロードエラー");
        }

      }
    }
  }
}

























