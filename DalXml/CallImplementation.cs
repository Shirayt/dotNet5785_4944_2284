
namespace Dal;
using DalApi;
using DO;
using System.Runtime.CompilerServices;

/// <summary>
/// Implementing the CRUD functions on the Call entity from XML data list
/// </summary>
internal class CallImplementation : ICall
{
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Create(Call item)
    {
        List<Call> Calls = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_calls_xml);
        Call newItem = item with { Id = Config.NextCallId };
        Calls.Add(newItem);
        XMLTools.SaveListToXMLSerializer(Calls, Config.s_calls_xml);
    }
    [MethodImpl(MethodImplOptions.Synchronized)]
    public Call? Read(int id)
    {
        List<Call> Calls = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_calls_xml);
        Call? existingItem = Calls.FirstOrDefault(call => call?.Id == id);

        if (existingItem != null)
        {
            return existingItem;
        }

        throw new DalDoesNotExistException($"Call with ID {id} does not exist.");
    }
    [MethodImpl(MethodImplOptions.Synchronized)]
    public Call? Read(Func<Call, bool> filter)
    {
        List<Call> Calls = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_calls_xml);
        Call? existingItem = Calls.FirstOrDefault(call => filter(call));

        if (existingItem != null)
        {
            return existingItem;
        }

        return null;
    }
    [MethodImpl(MethodImplOptions.Synchronized)]
    public IEnumerable<Call> ReadAll(Func<Call, bool>? filter = null)
    {
        List<Call> Calls = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_calls_xml);
        if (Calls.Count == 0)
        {
            return null;
        }
        return filter == null
            ? Calls.Select(item => item)
            : Calls.Where(filter);
    }
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Update(Call item)
    {
        List<Call> Calls = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_calls_xml);
        if (Calls.RemoveAll(it => it.Id == item.Id) == 0)
            throw new DalDoesNotExistException($"Call with ID={item.Id} does Not exist");
        Calls.Add(item);
        XMLTools.SaveListToXMLSerializer(Calls, Config.s_calls_xml);
    }
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Delete(int id)
    {
        List<Call> Calls = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_calls_xml);
        if (Calls.RemoveAll(it => it.Id == id) == 0)
            throw new DalDoesNotExistException($"Call with ID={id} does Not exist");
        XMLTools.SaveListToXMLSerializer(Calls, Config.s_calls_xml);
    }
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void DeleteAll()
    {
        XMLTools.SaveListToXMLSerializer(new List<Call>(), Config.s_calls_xml);
    }
}