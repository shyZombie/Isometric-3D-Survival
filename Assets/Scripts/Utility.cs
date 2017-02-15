using System;
using System.Collections;
using System.Collections.Generic;

public static class Utility 
{
    // Implements Fisher-Yates shuffle algorithm to randomize the positions of obstacles
    public static T[] ShuffleArray<T>(T[] array, int seed)
    {
        // Pseudorandom number generator var
        Random prng = new Random(seed);


        for (int i = 0; i < array.Length - 1; i++)
        {
            int randomIndex = prng.Next(i, array.Length);
            T tempItem = array[randomIndex];
            array[randomIndex] = array[i];
            array[i] = tempItem;
        }

        return array;
    }
}
