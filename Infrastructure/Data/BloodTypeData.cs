// Infrastructure/Seed/BloodTypeData.cs
public static class BloodTypeData
{
    private static readonly DateTime SeedCreatedOn = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    public static IEnumerable<BloodType> GetSeed() => new[]
    {
        new BloodType { BloodTypeId = 1, Name = "A+",  CreatedOn = SeedCreatedOn, CreatedBy = 1, IsDeleted = false },
        new BloodType { BloodTypeId = 2, Name = "A-",  CreatedOn = SeedCreatedOn, CreatedBy = 1, IsDeleted = false },
        new BloodType { BloodTypeId = 3, Name = "B+",  CreatedOn = SeedCreatedOn, CreatedBy = 1, IsDeleted = false },
        new BloodType { BloodTypeId = 4, Name = "B-",  CreatedOn = SeedCreatedOn, CreatedBy = 1, IsDeleted = false },
        new BloodType { BloodTypeId = 5, Name = "AB+", CreatedOn = SeedCreatedOn, CreatedBy = 1, IsDeleted = false },
        new BloodType { BloodTypeId = 6, Name = "AB-", CreatedOn = SeedCreatedOn, CreatedBy = 1, IsDeleted = false },
        new BloodType { BloodTypeId = 7, Name = "O+",  CreatedOn = SeedCreatedOn, CreatedBy = 1, IsDeleted = false },
        new BloodType { BloodTypeId = 8, Name = "O-",  CreatedOn = SeedCreatedOn, CreatedBy = 1, IsDeleted = false },
    };
}
