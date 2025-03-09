using System.Collections.Generic;
using Brain.Tools;

namespace Brain;

public interface IChatProvider<T> where T : ITool
{
    public T[] Tools { get; }
    
    // public Queue<T> CallQueue { get; }
    //
    // private Task 
}