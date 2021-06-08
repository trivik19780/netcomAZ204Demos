using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System;
using System.IO;
using System.Threading.Tasks;
namespace sademo
{
    class Program
    {
        public static void Main()
        {
            Console.WriteLine("Azure Blob Storage Demo\n");
            ProcessAsync().GetAwaiter().GetResult();
            Console.WriteLine("Press enter to exit the sample application.");
            Console.ReadLine();
        }
        private static async Task ProcessAsync()
        {
            string storageConnectionString = "DefaultEndpointsProtocol=https;AccountName=az203sademo;AccountKey=tf9plcYsqvCQxAe1R96Den7142jqB4F6SlegdyTN+xJfOld3Uu+Zh35JXK6r10tJ1o+zCf/hLejB2x9N5GrFIg==;EndpointSuffix=core.windows.net";
            BlobServiceClient blobServiceClient = new BlobServiceClient(storageConnectionString);
            
            //Create a unique name for the container
            string containerName = "demoblob" + Guid.NewGuid().ToString();
            // Create the container and return a container client object
            BlobContainerClient containerClient = await blobServiceClient.CreateBlobContainerAsync(containerName);
            Console.WriteLine("A container named '" + containerName + "' has been created. " +  "\nTake a minute and verify in the portal." +
            "\nNext a file will be created and uploaded to the container.");
            Console.WriteLine("Press 'Enter' to continue.");
            Console.ReadLine();
            //uploading data to blob created//
            // Create a local file in the ./azdata/ directory for uploading and downloading
            string localPath = "../../../azdata/";
            string fileName = "demofile" + Guid.NewGuid().ToString() + ".txt";
            string localFilePath = Path.Combine(localPath, fileName);
            // Write text to the file
            await File.WriteAllTextAsync(localFilePath, "Hello, World!");
            // Get a reference to the blob
            BlobClient blobClient = containerClient.GetBlobClient(fileName);
            Console.WriteLine("Uploading to Blob storage as blob:\n\t {0}\n", blobClient.Uri);
            // Open the file and upload its data
            using FileStream uploadFileStream = File.OpenRead(localFilePath);
            await blobClient.UploadAsync(uploadFileStream, true);
            uploadFileStream.Close();
            Console.WriteLine("\nThe file was uploaded. We'll verify by listing" + " the blobs next.");
            Console.WriteLine("Press 'Enter' to continue.");
            Console.ReadLine();

            // List blobs in the container
            Console.WriteLine("Listing blobs...");
            await foreach (BlobItem blobItem in containerClient.GetBlobsAsync())
            {
                Console.WriteLine("\t" + blobItem.Name);
            }
            Console.WriteLine("\nYou can also verify by looking inside the " +
            "container in the portal." +   "\nNext the blob will be downloaded with an altered file name.");
            Console.WriteLine("Press 'Enter' to continue.");
            Console.ReadLine();

            // Download the blob to a local file
            // Append the string "DOWNLOADED" before the .txt extension
            string downloadFilePath = localFilePath.Replace(".txt", "DOWNLOADED.txt");
            Console.WriteLine("\nDownloading blob to\n\t{0}\n", downloadFilePath);
            // Download the blob's contents and save it to a file
            BlobDownloadInfo download = await blobClient.DownloadAsync();
            using (FileStream downloadFileStream = File.OpenWrite(downloadFilePath))
            {
                await download.Content.CopyToAsync(downloadFileStream);
                downloadFileStream.Close();
            }
            Console.WriteLine("\nLocate the local file to verify it was downloaded.");
            //Console.WriteLine("The next step is to delete the container and local  files.");
            Console.WriteLine("Press 'Enter' to continue.");
            Console.ReadLine();

        }
    }
}