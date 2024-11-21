namespace DO;

public record Assignment(int TaskId, string TaskName, DateTime DueDate, bool IsCompleted = false)
{
    bool IsCompleted = false;

    // Constructor and methods

    public void CompleteTask()
    {
        IsCompleted = true;
    }

}