
// See https://aka.ms/new-console-template for more information
using Bockchain_Implementation;

Console.WriteLine("Hello, World!");

var rand = new Random();
IBlock genesis = new Block(new byte[] { 0x00, 0x00, 0x00, 0x00 });
byte[] difficulty = new byte[] { 0x00, 0x00 };
Blockchain chain = new Blockchain(difficulty, genesis);
for (int i = 0; i < 200; i++)
{
    var data = Enumerable.Range(0, 255).Select(p => (byte)rand.Next(255));
    chain.Add(new Block(data.ToArray()));
    Console.WriteLine(chain.LastOrDefault()?.ToString());
    if (chain.IsBlockValid())
        Console.WriteLine("blockchain is valid");
}
Console.ReadLine();
