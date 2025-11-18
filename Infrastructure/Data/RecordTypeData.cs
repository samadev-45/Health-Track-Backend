using System;
using System.Collections.Generic;
using Health.Domain.Entities;

namespace Health.Infrastructure.Data
{
    public static class RecordTypeData
    {
        private static readonly DateTime SeedCreatedOn =
            new DateTime(2024, 01, 01, 0, 0, 0, DateTimeKind.Utc);

        public static IEnumerable<RecordType> GetSeed() => new[]
        {
            new RecordType { RecordTypeId = 1, Name = "Prescription",   CreatedOn = SeedCreatedOn, CreatedBy = 1, IsDeleted = false },
            new RecordType { RecordTypeId = 2, Name = "Lab Report",     CreatedOn = SeedCreatedOn, CreatedBy = 1, IsDeleted = false },
            new RecordType { RecordTypeId = 3, Name = "X-Ray",          CreatedOn = SeedCreatedOn, CreatedBy = 1, IsDeleted = false },
            new RecordType { RecordTypeId = 4, Name = "MRI",            CreatedOn = SeedCreatedOn, CreatedBy = 1, IsDeleted = false },
            new RecordType { RecordTypeId = 5, Name = "Scan",           CreatedOn = SeedCreatedOn, CreatedBy = 1, IsDeleted = false },
            new RecordType { RecordTypeId = 6, Name = "Other",          CreatedOn = SeedCreatedOn, CreatedBy = 1, IsDeleted = false },
        };
    }
}
