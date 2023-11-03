// See https://aka.ms/new-console-template for more information
using Blackbird.Plugins.Plunet;

Console.WriteLine("Hello, World!");

var url = "https://mothertongue.plunet.com";
var username = "Blackbird Service";
var password = "sDb4";

using var plunetApiClient = Clients.GetAuthClient(url);
var uuid = plunetApiClient.loginAsync(username, password)
    .GetAwaiter().GetResult();

Console.WriteLine(uuid);