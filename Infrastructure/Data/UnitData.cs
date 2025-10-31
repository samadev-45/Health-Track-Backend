// Seed/UnitData.cs
using System;
using System.Collections.Generic;
using Health.Domain.Entities;

namespace Health.Infrastructure.Data
{
    public static class UnitData
    {
        
        private static readonly DateTime SeedCreatedOn = new DateTime(2024, 01, 01, 0, 0, 0, DateTimeKind.Utc);

        public static IEnumerable<Unit> GetSeedUnits() => new[]
        {
            new Unit { UnitId = 1, UnitName = "mg",  Description = "milligram",          CreatedOn = SeedCreatedOn, CreatedBy = 1, IsDeleted = false },
            new Unit { UnitId = 2, UnitName = "g",   Description = "gram",               CreatedOn = SeedCreatedOn, CreatedBy = 1, IsDeleted = false },
            new Unit { UnitId = 3, UnitName = "mcg", Description = "microgram",          CreatedOn = SeedCreatedOn, CreatedBy = 1, IsDeleted = false },
            new Unit { UnitId = 4, UnitName = "ml",  Description = "milliliter",         CreatedOn = SeedCreatedOn, CreatedBy = 1, IsDeleted = false },
            new Unit { UnitId = 5, UnitName = "L",   Description = "liter",              CreatedOn = SeedCreatedOn, CreatedBy = 1, IsDeleted = false },
            new Unit { UnitId = 6, UnitName = "IU",  Description = "international unit", CreatedOn = SeedCreatedOn, CreatedBy = 1, IsDeleted = false },
        };
    }
}
