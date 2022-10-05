using System.Globalization;
using CsvHelper;

namespace PetAdoption
{
    /// <summary>
    /// Static methods for helping parse pets out of knwon file formats
    /// </summary>
    internal static class PetParser
    {
        /// <summary>
        /// Parses a collection of <see cref="Pet"/> objects from a csv file
        /// </summary>
        /// <param name="path">Path to the file</param>
        /// <param name="pets">Output list of pets</param>
        public static void ParsePetsCsv(string path, out List<Pet> pets)
        {
            pets = new List<Pet>();

            using (var reader = new StreamReader(path))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                csv.Read();
                csv.ReadHeader();
                while (csv.Read())
                {
                    var pet = new Pet();
                    for (var i = 0; i < (csv.HeaderRecord?.Length ?? 0); i++)
                    {
                        var header = csv.HeaderRecord[i];
                        var value = csv[i].Trim();
                        switch (header.Trim().ToLowerInvariant())
                        {
                            case "id":
                                if (int.TryParse(value, out var id))
                                {
                                    pet.Id = id;
                                }

                                break;

                            case "name":
                                pet.Name = value;
                                break;

                            case "type":
                                if (Enum.TryParse<PetType>(value.ToLower(), out var type))
                                {
                                    pet.Type = type;
                                }

                                break;

                            case "gender":
                                switch (value.ToLowerInvariant())
                                {
                                    case "neuter":
                                    case "neutered":
                                        pet.Gender = "Male";
                                        pet.Fixed = true;
                                        break;

                                    case "spayed":
                                    case "spay":
                                        pet.Gender = "Female";
                                        pet.Fixed = true;
                                        break;

                                    default:
                                        pet.Gender = value;

                                        // Given the sample data we saw, gonna assume this means pet is not fixed, but this could vary by source, may be better just leave as null
                                        pet.Fixed = false;

                                        break;
                                }

                                break;

                            case "zipcode":
                                if (int.TryParse(value, out var zip))
                                {
                                    pet.ZipCode = zip;
                                }
                                
                                break;

                            default:
                                // Gonna do nothing, would preferably log something or communicate to user that their input file has unexpected header,
                                // but going to consider this out of scope for the exercise. Similarly, would ideally do so for any above TryParses that failed
                                break;
                        }
                    }

                    pets.Add(pet);
                }
            }
        }
    }
}
