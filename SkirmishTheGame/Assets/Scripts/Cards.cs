using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Security.Cryptography;


// this is what a card is 

// this is the class and it only contains the suit and value which are enums
[System.Serializable]
public class Cards
{

    public int value;

    public Cards(int value)
    {
        this.value = value;
    }

    public Cards() { }

}





[System.Serializable]
public class Deck
{

    public List<Cards> cards = new List<Cards>();


    //make a new default contriutcrto because its stupid
    public Deck()
    {
        cards = new List<Cards>();
        ClearDeck();
    }



    public void ClearDeck() { cards.Clear(); }


    public void Shuffle()
    {
        RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider();
        int n = cards.Count;
        while (n > 1)
        {
            byte[] box = new byte[1];
            do provider.GetBytes(box);
            while (!(box[0] < n * (byte.MaxValue / n)));
            int k = (box[0] % n);
            n--;
            Cards value = cards[k];
            cards[k] = cards[n];
            cards[n] = value;
        }
    }



    public Cards GetCard()
    {
        Cards card = cards[0];
        cards.RemoveAt(0);
        return card;
    }



}






