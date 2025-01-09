using DalApi;
namespace Helpers;

/// <summary>
/// Internal static helper class for all call data
/// </summary>
internal static class CallManager
{
    private static IDal s_dal = Factory.Get; //stage 4
}
