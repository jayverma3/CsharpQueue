using System;
using System.Collections;
using System.Collections.Generic;

namespace CsharpQueue { 
public class Document
{
    private string title;
    public string Title
    { 
        get
        {
            return title;
        }
    }

    private string content;
    public string Content
    {
        get
        {
        return content;
        }
    }

    public Document(string title, string content)
    {
    this.title = title;
    this.content = content;
    }
}

public class DocumentManager
    {
        private readonly Queue<Document> documentQueue = new Queue<Document>();

        public void AddDocument(Document doc)
        {
            lock (this)
            {
                documentQueue.Enqueue(doc);
            }
        }

        public Document GetDocument()
        {
            Document doc = null;
            lock (this)
            {
                doc = documentQueue.Dequeue();
            }
            return doc;
        }

        public bool IsDocumentAvailable
        {
            get
            {
                return documentQueue.Count > 0;
            }
        }
    }

public class ProcessDocuments
    {
        protected ProcessDocuments(DocumentManager dm)
        {
            DocumentManager = dm;
        }

        public static void Start(DocumentManager dm)
        {
            new Thread(new ProcessDocuments(dm).Run).Start();
        }

        private DocumentManager documentManager;

        public DocumentManager DocumentManager { get; }

        protected void Run()
        {
            while (true)
            {
                if (documentManager.IsDocumentAvailable)
                {
                    Document doc = documentManager.GetDocument();
                    Console.WriteLine("Processing Document {0}",doc.Title);
                }
                Thread.Sleep(new Random().Next(20));
            }
        }
    }

class Program
    {
        static void Main()
        {
            DocumentManager dm = new DocumentManager();

            ProcessDocuments.Start(dm);

            //Create Docs and add them to the Document manager
            for (int i = 0; i < 1000; i++)
            {
                Document doc = new Document("Doc" + i.ToString(), "content");
                dm.AddDocument(doc);
                Console.WriteLine("Added document {0}", doc.Title);
                Thread.Sleep(new Random().Next(20));
            }
        }
    }
}