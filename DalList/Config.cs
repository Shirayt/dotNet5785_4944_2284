﻿namespace Dal;

internal static class Config
{
    internal const int initialCallId = 0;

    internal const int initialAssignmentId = 0;

    private static int CallId = initialCallId;

    private static int AssignmentId = initialAssignmentId;
    internal static int NextCallId { get => CallId++; }
    internal static int NextAssignmentId { get => AssignmentId++; }
    public static DateTime Clock { get; set; }
    static TimeSpan RiskRange { get; set; }

    internal static void Reset()
    {
        CallId = initialCallId;

        AssignmentId = initialAssignmentId;

        Clock = DateTime.Now;

        RiskRange= TimeSpan.Zero;
    }

}