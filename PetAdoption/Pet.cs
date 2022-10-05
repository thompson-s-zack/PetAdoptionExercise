namespace PetAdoption
{
    /// <summary>
    /// Represents a pet available for adoption
    /// </summary>
    internal class Pet
    {
        public int? Id { get; set; }

        public string? Name { get; set; }

        public PetType Type { get; set; }

        public string? Gender { get; set; }

        public bool? Fixed { get; set; } = null;

        public int? ZipCode { get; set; }

        public override string ToString()
        {
            return $"Id:{Id}, Name:{Name}, Type:{Type}, Gender:{Gender}, Fixed:{Fixed}, ZipCode:{ZipCode}";
        }

    }
}
