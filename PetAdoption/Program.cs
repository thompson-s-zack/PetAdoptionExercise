namespace PetAdoption
{
    internal class Program
    {
        /// <summary>
        /// Driver of the program
        /// </summary>
        /// <param name="args">Unused cli arguments</param>
        static void Main(string[] args)
        {
            var quit = false;
            var pets = new List<Pet>();
            while (!quit)
            {
                Console.WriteLine("Choose an option:");
                Console.WriteLine("1 - Bulk add pets");
                Console.WriteLine("2 - Search pets by location");
                Console.WriteLine("3 - Search pets by type");
                Console.WriteLine("4 - Search pets by combination");
                Console.WriteLine("5 - Exit");
                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        Console.Write("Path to pet file: ");
                        // Adding blank as location on my machine (for laziness' sake)
                        var path = Console.ReadLine();
                        path = string.IsNullOrWhiteSpace(path) ? @"D:\code\C#\PetAdoption\PetAdoption\Test Data\pets.csv" : path;
                        PetParser.ParsePetsCsv(path, out var newPets);
                        pets.AddRange(newPets);
                        break;

                    case "2":
                        Console.Write("Enter zip code: ");
                        var zip = Console.ReadLine();
                        ShowFilteredPets(pets, $"zipcode:{zip}");
                        break;

                    case "3":
                        Console.Write("Enter pet type: ");
                        var type = Console.ReadLine();
                        ShowFilteredPets(pets, $"type:{type}");
                        break;

                    case "4":
                        Console.WriteLine("Format filters as \"<filter term>:<filter value>\". Separate multiple filters with a comma.");
                        Console.WriteLine("Example filter input: location:90210, type:dog");
                        Console.Write("Enter filter(s): ");
                        var filters = Console.ReadLine();
                        ShowFilteredPets(pets, filters);
                        break;

                    case "5":
                        quit = true;
                        break;

                    default:
                        Console.WriteLine("Invalid option");
                        break;
                }
            }
        }

        /// <summary>
        /// Dispay a list of pets that match one or more filters
        /// </summary>
        /// <param name="pets">The pets to search</param>
        /// <param name="filterInput">The filter(s) to be applied. Formatted as comma separated list of filter filterType:filterValue</param>
        private static void ShowFilteredPets(List<Pet> pets, string filterInput)
        {
            var filteredPets = new List<Pet>(pets);
            var filters = filterInput.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.RemoveEmptyEntries);
            foreach (var filter in filters)
            {
                var parts = filter.Split(":");
                if (!(parts.Length == 2 && TryFilterPets(filteredPets, parts[0], parts[1], out filteredPets)))
                {
                    Console.WriteLine($"Filter \"{filter}\" is incorrectly formatted, and will not be used");
                }
            }

            if (filteredPets.Count == 0)
            {
                Console.WriteLine("No pets matched filter criteria");
            }
            else
            {
                Console.WriteLine("Matching pets:");
                foreach (var pet in filteredPets)
                {
                    Console.WriteLine(pet);
                }
            }
        }

        /// <summary>
        /// Filter list of pets with one provided filter
        /// </summary>
        /// <param name="pets">List of pets</param>
        /// <param name="key">Pet's property filtering by</param>
        /// <param name="value">Value of the pet's property to look for</param>
        /// <param name="filteredPets">The filtered list of pets</param>
        /// <returns>True if the filter was valid and able to be applied. This makes no guarantees of valid value argument or if > 0 pets will be returned</returns>
        private static bool TryFilterPets(List<Pet> pets, string key, string value, out List<Pet> filteredPets)
        {
            filteredPets = new List<Pet>();
            Func<Pet, bool> predicate = null;
            switch (key.Trim().ToLower())
            {
                case "id":
                    predicate = pet => int.TryParse(value, out int id) && pet.Id == id;
                    break;

                case "name":
                    predicate = pet => string.Compare(pet.Name, value, StringComparison.OrdinalIgnoreCase) == 0;
                    break;

                case "type":
                    predicate = pet => Enum.TryParse<PetType>(value.ToLower(), out var type) && pet.Type == type;
                    break;

                case "gender":
                    predicate = pet =>
                    {
                        switch (value.ToLowerInvariant())
                        {
                            case "neuter":
                            case "neutered":
                                return pet.Fixed ?? false && (string.Compare(pet.Gender, value, StringComparison.OrdinalIgnoreCase) == 0 || string.Compare(pet.Gender, "male", StringComparison.OrdinalIgnoreCase) == 0);

                            case "spayed":
                            case "spay":
                                return pet.Fixed ?? false && (string.Compare(pet.Gender, value, StringComparison.OrdinalIgnoreCase) == 0 || string.Compare(pet.Gender, "female", StringComparison.OrdinalIgnoreCase) == 0);

                            default:
                                return string.Compare(pet.Gender, value, StringComparison.OrdinalIgnoreCase) == 0;
                        }
                    };

                    break;

                case "fixed":
                    predicate = pet => bool.TryParse(value, out var isFixed) && isFixed == pet.Fixed;
                    break;

                case "zipcode":
                case "location":
                    predicate = pet => int.TryParse(value, out var zip) && pet.ZipCode == zip;
                    break;

                default:
                    return false;
            }

            filteredPets = pets.Where(predicate).ToList();

            return true;
        }
    }
}