using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordPoker {

    public class Deck {

        private Random rand = new Random();
        private List<Card> Used = new List<Card>();
        private List<Card> Unused = new List<Card>();
        public int CardCount { get { return Used.Count + Unused.Count; } }

        public Deck(int decks, bool shuffle, bool jokers) {
            if (decks <= 0) { return; }
            Unused = GenerateDeck(decks, jokers);
            if (shuffle) { Shuffle(); }
        }

        public List<Card> GenerateDeck(int decks, bool jokers) {
            List<Card> deck = new List<Card>();
            foreach (Suit s in Enum.GetValues(typeof(Suit))) {
                if (s == Suit.NULL || s == Suit.Joker) { continue; }
                foreach (Rank r in Enum.GetValues(typeof(Rank))) {
                    if (r == Rank.NULL || r == Rank.Joker) { continue; }
                    deck.Add(new Card(r, s));
                }
            }
            if (jokers) {
                deck.Add(new Card(Rank.Joker, Suit.Joker));
                deck.Add(new Card(Rank.Joker, Suit.Joker));
            }
            return deck;
        }

        public void Shuffle() {
            Used.AddRange(Unused);
            Unused.Clear();
            while (Used.Count > 0) {
                int next = rand.Next(0, Used.Count);
                Unused.Add(Used[next]);
                Used.RemoveAt(next);
            }
        }

        public List<Card> DrawCards(int num) {
            List<Card> cards = new List<Card>();
            if (num > Unused.Count) { 
                cards.AddRange(Unused);
                Shuffle();
                Used.AddRange(cards);
                foreach (Card c in cards) { Unused.Remove(c); }
            }
            if (num > Unused.Count) { throw new Exception("Insufficent number of cards."); }
            int more = num - cards.Count;
            cards.AddRange(Unused.GetRange(0, more));
            Used.AddRange(Unused.GetRange(0, more));
            Unused.RemoveRange(0, more);
            return cards;
        }
    }
}
