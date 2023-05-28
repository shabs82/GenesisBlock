using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Bockchain_Implementation
{

    public interface IBlock
    {
        public byte[] Data { get; }
        public byte[] Hash { get; set; }
        public int Nonce { get; set; }
        public byte[] PrevHash { get; set; }
        public DateTime TimeStamp { get; set; }
    }
    public class Block : IBlock
    {
        public Block(byte[] data)
        {
            Data = data ?? throw new ArgumentNullException(nameof(data));
            Nonce = 0;
            PrevHash = new byte[data.Length];
            TimeStamp = DateTime.Now;
        }

        public byte[] Data { get; }
        public byte[] Hash { get; set; }
        public int Nonce { get; set; }
        public byte[] PrevHash { get; set; }
        public DateTime TimeStamp { get; set; }

        public override string ToString()
        {
            return $"{BitConverter.ToString(Hash).Replace("-", "")} \n {BitConverter.ToString(PrevHash).Replace("-", "")} \n{Nonce} {TimeStamp}";

        }
    }

    public static class BlockExtension
    {
        public static byte[] GenerateHash(this IBlock block)
        {
            using (SHA512 sha512 = new SHA512Managed())
            using (MemoryStream ms = new MemoryStream())
            using (BinaryWriter bw = new BinaryWriter(ms))

            {
                bw.Write(block.Data);
                bw.Write(block.Nonce);
                bw.Write(block.PrevHash.Length);
                bw.Write(block.TimeStamp.ToString());

                var s = ms.ToArray();
                return sha512.ComputeHash(s);
            }
        }

        public static byte[] MineHash(this IBlock block, byte[] difficulty)
        {
            if (difficulty == null)
                throw new ArgumentNullException(nameof(difficulty));
            byte[] hash = new byte[0];

            while (hash.Take(2).SequenceEqual(difficulty))
            {
                block.Nonce++;
                hash = block.GenerateHash();
            }
            return hash;

        }

        public static bool IsValid(this IBlock block)
        {
            var bk = block.GenerateHash();
            return block.Hash.SequenceEqual(bk);
        }

        public static bool IsPrevBlockValid(this IBlock block, IBlock prevBlock)
        {
            if (prevBlock == null)
                throw new ArgumentNullException(nameof(prevBlock));
            return prevBlock.IsValid() && block.PrevHash.SequenceEqual((prevBlock).Hash);
        }

        public static bool IsBlockValid(this IEnumerable<IBlock> items)

        {
            var enums = items.ToList();
            return enums.Zip(enums.Skip(1), Tuple.Create).All
                (block => block.Item2.IsValid() && block.Item2.IsPrevBlockValid(block.Item1));
        }



    }

    public class Blockchain : IEnumerable<IBlock>
    {
        private List<IBlock> _items = new List<IBlock>();
        public byte[] Difficulty { get; set; }

        public Blockchain(byte[] _difficulty, IBlock genesis)
        {
            Difficulty = _difficulty;
            genesis.Hash = genesis.MineHash(_difficulty);
            Items.Add(genesis);
        }

        public void Add(IBlock item)
        {
            if (_items.LastOrDefault() != null)
            {
                item.PrevHash = _items.LastOrDefault().Hash;
            }
            item.Hash = item.MineHash(Difficulty);
            Items.Add(item);
        }

        public List<IBlock> Items
        {
            get { return _items; }
            set { _items = value; }
        }

        public int Count { get { return Items.Count; } }
        public IBlock this[int index]
        {
            get { return Items[index]; }
            set { Items[index] = value; }
        }

        IEnumerator<IBlock> IEnumerable<IBlock>.GetEnumerator()
        {
            return Items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Items.GetEnumerator();
        }
    }
}