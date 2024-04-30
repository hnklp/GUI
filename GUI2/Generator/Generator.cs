using Bogus;
using System.Collections.Generic;
using EFCore.Data;

namespace EFCore.Generator
{
    class DataGenerator
    {
        public static IList<Mob> GenerateMobs(int count)
        {
            Faker<Mob> generator = new Faker<Mob>()
                .StrictMode(true)
                .RuleFor(x => x.MobId, f => f.IndexGlobal)
                .RuleFor(x => x.Name, f => f.Person.FirstName)
                .RuleFor(x => x.DateOfCapture, f => f.Person.DateOfBirth)
                .RuleFor(x => x.SpeciesId, f => 1)
                .RuleFor(x => x.Species, f => null);

            return generator.Generate(count);
        }
    }
}
