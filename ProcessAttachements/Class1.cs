
using System;

public class Rootobject
{
    public Attachment[] attachments { get; set; }
    public string body { get; set; }
    public string subject { get; set; }
}

public class Attachment
{
    public string Id { get; set; }
    public string Name { get; set; }
    public byte[] ContentBytes { get; set; }
    public string ContentType { get; set; }
    public int Size { get; set; }
    public bool IsInline { get; set; }
    public DateTime LastModifiedDateTime { get; set; }
    public string ContentId { get; set; }
}
