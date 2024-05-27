using System.Runtime.Intrinsics.X86;
using Amazon.SecurityToken.Model;
using Bam;
using GitJira.Interfaces;

namespace GitJira.Classes;

public class GitSettings : IGitSettings
{
    public string Owner { get; set; }
    public string Repository { get; set; }
    public string ProductHeader { get; set;}
    public string AuthToken { get; set; }


}