using System;
using System.Drawing;
using System.Collections.Generic;
using System.Text;

namespace DiscordPoker {

    public enum Rank {
        NULL = 0, Two = 2, Three = 3, Four = 4, Five = 5, Six = 6, Seven = 7, Eight = 8,
        Nine = 9, Ten = 10, Jack = 11, Queen = 12, King = 13, Ace = 14, Joker = 15
    };

    public enum Suit { NULL, Clubs, Spades, Hearts, Diamonds, Joker };

    public class Card {

        public static Size CardSize = new Size(76, 108);

        public Rank Rank { get; private set; }
        public Suit Suit { get; private set; }
        public Bitmap Back { get; private set; }
        public Bitmap Front { get; private set; }

        private static Dictionary<Suit, int> SuitToIndex = new Dictionary<Suit, int>() {
            { Suit.Spades, 0 }, { Suit.Diamonds, 1 }, { Suit.Hearts, 2 }, { Suit.Clubs, 3 }
        };

        private static Dictionary<Rank, int> RankToIndex = new Dictionary<Rank, int>() {
            { Rank.Ace, 0 }, { Rank.Two, 1 }, { Rank.Three, 2 }, { Rank.Four, 3 }, { Rank.Five, 4 },
            { Rank.Six, 5 }, { Rank.Seven, 6 }, { Rank.Eight, 7 }, { Rank.Nine, 8 }, { Rank.Ten, 9 },
            { Rank.Jack, 10 }, { Rank.Queen, 11 }, { Rank.King, 12 }
        };

        private static Dictionary<Rank, Tuple<Rank, Rank>> RankNeighbors = new Dictionary<Rank, Tuple<Rank, Rank>>() {
            { Rank.Ace,   new Tuple<Rank, Rank>(Rank.Two,   Rank.King  ) },
            { Rank.Two,   new Tuple<Rank, Rank>(Rank.Ace,   Rank.Three ) },
            { Rank.Three, new Tuple<Rank, Rank>(Rank.Two,   Rank.Four  ) },
            { Rank.Four,  new Tuple<Rank, Rank>(Rank.Three, Rank.Five  ) },
            { Rank.Five,  new Tuple<Rank, Rank>(Rank.Four,  Rank.Six   ) },
            { Rank.Six,   new Tuple<Rank, Rank>(Rank.Five,  Rank.Seven ) },
            { Rank.Seven, new Tuple<Rank, Rank>(Rank.Six,   Rank.Eight ) },
            { Rank.Eight, new Tuple<Rank, Rank>(Rank.Seven, Rank.Nine  ) },
            { Rank.Nine,  new Tuple<Rank, Rank>(Rank.Eight, Rank.Ten   ) },
            { Rank.Ten,   new Tuple<Rank, Rank>(Rank.Nine,  Rank.Jack  ) },
            { Rank.Jack,  new Tuple<Rank, Rank>(Rank.Ten,   Rank.Queen ) },
            { Rank.Queen, new Tuple<Rank, Rank>(Rank.Jack,  Rank.King  ) },
            { Rank.King,  new Tuple<Rank, Rank>(Rank.Queen, Rank.Ace   ) },
        };

        public Card(Rank rank, Suit suit) {
            Front = new Bitmap(CardSize.Width, CardSize.Height);
            Back = new Bitmap(CardSize.Width, CardSize.Height);
            Rank = rank;
            Suit = suit;
            DrawCard();
        }

        public void DrawCard() {
            int index = SuitToIndex[Suit] * 13 + RankToIndex[Rank];
            Rectangle src = new Rectangle((index % 10) * 204, (index / 10) * 288, 204, 288);
            Rectangle dst = new Rectangle(new Point(0, 0), CardSize);
            Graphics graph = Graphics.FromImage(Front);
            graph.DrawImage(DiscordPoker.Properties.Resources.Cards, dst, src, GraphicsUnit.Pixel);
            graph.Dispose();
            Rectangle bsrc = new Rectangle(7 * 204, 5 * 288, 204, 288);
            Graphics bgraph = Graphics.FromImage(Back);
            bgraph.DrawImage(DiscordPoker.Properties.Resources.Cards, dst, bsrc, GraphicsUnit.Pixel);
            bgraph.Dispose();
        }

        public Tuple<Rank, Rank> NeighborRanks() {
            RankNeighbors.TryGetValue(this.Rank, out Tuple<Rank, Rank> ranks);
            return ranks;
        }

        public static bool operator >(Card card1, Card card2) { return card1.Rank > card2.Rank; }

        public static bool operator <(Card card1, Card card2) { return card1.Rank < card2.Rank; }

        public static bool operator ==(Card card1, Card card2) { return card1.Rank == card2.Rank; }

        public static bool operator !=(Card card1, Card card2) { return card1.Rank != card2.Rank; }

        public static Card Compare(Card card1, Card card2) {
            return (card1.Rank > card2.Rank) ? card1 : (card2.Rank > card1.Rank) ? card2 : new Card(card1.Rank, Suit.NULL);
        }

        public static Bitmap DrawCards(List<Card> cards) {
            Bitmap canvas = new Bitmap(CardSize.Width * cards.Count, CardSize.Height);
            Graphics graph = Graphics.FromImage(canvas);
            Rectangle src = new Rectangle(new Point(0, 0), CardSize);
            for (int i = 0; i < cards.Count; i++) {
                Rectangle dst = new Rectangle(i * CardSize.Width, 0, CardSize.Width, CardSize.Height);
                graph.DrawImage(cards[i].Front, dst, src, GraphicsUnit.Pixel);
            }
            graph.Dispose();
            return canvas;
        }

        public bool IsNull() {
            return (Rank == Rank.NULL || Suit == Suit.NULL);
        }
    }
}
