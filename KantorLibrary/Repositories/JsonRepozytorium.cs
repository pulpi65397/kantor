using KantorLibrary.Repositories;
using Newtonsoft.Json;

public class JsonRepozytorium<T> : IRepozytorium<T> where T : class
{
    private readonly string _filePath;
    private List<T> _items;

    public JsonRepozytorium(string filePath)
    {
        _filePath = filePath;
        _items = LoadFromFile();
    }

    private List<T> LoadFromFile()
    {
        if (File.Exists(_filePath))
        {
            var json = File.ReadAllText(_filePath);
            return JsonConvert.DeserializeObject<List<T>>(json) ?? new List<T>();
        }
        return new List<T>();
    }

    private void SaveToFile()
    {
        var json = JsonConvert.SerializeObject(_items, Formatting.Indented);
        File.WriteAllText(_filePath, json);
    }

    public IEnumerable<T> GetAll()
    {
        return _items;
    }

    public T GetById(int id)
    {
        return _items.FirstOrDefault(item => (item as dynamic).Id == id);
    }

    public void Add(T item)
    {
        _items.Add(item);
        SaveToFile();
    }

    public void Update(T item)
    {
        var id = (item as dynamic).Id;
        var existingItem = _items.FirstOrDefault(i => (i as dynamic).Id == id);
        if (existingItem != null)
        {
            var index = _items.IndexOf(existingItem);
            _items[index] = item;
            SaveToFile();
        }
    }

    public void Delete(int id)
    {
        var item = _items.FirstOrDefault(i => (i as dynamic).Id == id);
        if (item != null)
        {
            _items.Remove(item);
            SaveToFile();
        }
    }

    // Implementacja metody ToList()
    public List<T> ToList()
    {
        return _items.ToList();
    }
}
