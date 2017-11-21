using Microsoft.WindowsAzure.Storage;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;

namespace MultiStream.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            var storageConnection = ConfigurationManager.AppSettings["connection"];

            var storageContainer = ConfigurationManager.AppSettings["container"];

            var storageAccount = CloudStorageAccount.Parse(storageConnection);

            var blobClient = storageAccount.CreateCloudBlobClient();

            var container = blobClient.GetContainerReference(storageContainer);

            var blobs = container.ListBlobs();

            var streams = new List<Stream>();

            foreach (var blobRef in blobs)
            {
                var blob = blobClient.GetBlobReferenceFromServer(blobRef.StorageUri);
                streams.Add(blob.OpenRead());
            }

            var ms = new Lib.MultiStream(streams);

            var reader = new StreamReader(ms);

            Console.WriteLine(reader.ReadToEnd());
        }
    }
}
