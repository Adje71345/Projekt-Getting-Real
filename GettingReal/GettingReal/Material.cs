public class Material
{
    public string Category { get; set; }
    public string Description { get; set; }
    public int Amount { get; set; }

    public Material(string category, string description, int amount)
    {
        Category = category;
        Description = description;
        Amount = amount;
    }

    public override string ToString()
    {
        return $"Category: {Category}, Description: {Description}, Amount: {Amount}";
    }
}