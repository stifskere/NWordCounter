namespace NwordCounter.Stuff;

public static class Algorithms
{
    public static string AddSuffix(this int number) 
        => number % 100 >= 11 && number % 100 <= 13 ? number + "th" : number + (number % 10 == 1 ? "st" : number % 10 == 2 ? "nd" : number % 10 == 3 ? "rd" : "th");
}