using InfFloorRemaster.Classes;

namespace InfFloorRemaster
{
    public static class UpgradeRegister
    {
        private static void Add(StandardUpgrade upgrade)
        {
            InfFloorMod.Upgrades.Add(upgrade.id, upgrade); 
        }

        internal static void RegisterDefaults()
        {
            Add(new StandardUpgrade()
            {
                id = "AutoTag",
                weight = 120,
                levels = new UpgradeLevel[2]
                {
                    new UpgradeLevel()
                    {
                        cost = 125,
                        descLoca = "Auto tag - When the level starts, the tag is put on automatically.",
                        icon = "AutoTag"
                    },
                    new UpgradeLevel()
                    {
                        cost = 250,
                        descLoca = "Auto tag - When the level starts, the tag is put on automatically.",
                        icon = "AutoTag2"
                    }
                }
            });
            Add(new StandardUpgrade()
            {
                id = "Bank",
                weight = 120,
                levels = new UpgradeLevel[6]
                {
                    new UpgradeLevel()
                    {
                        cost = 125,
                        descLoca = "When an item disappears from your inventory, a coin may appear in this slot",
                        icon = "Bank1"
                    },
                    new UpgradeLevel()
                    {
                        cost = 250,
                        descLoca = "When an item disappears from your inventory, a coin may appear in this slot",
                        icon = "Bank2"
                    },
                    new UpgradeLevel()
                    {
                        cost = 750,
                        descLoca = "When an item disappears from your inventory, a coin may appear in this slot",
                        icon = "Bank3"
                    },
                    new UpgradeLevel()
                    {
                        cost = 1000,
                        descLoca = "When an item disappears from your inventory, a coin may appear in this slot",
                        icon = "Bank5"
                    },
                    new UpgradeLevel()
                    {
                        cost = 110,
                        descLoca = "When an item disappears from your inventory, a coin may appear in this slot",
                        icon = "Bank6"
                    },
                    new UpgradeLevel()
                    {
                        cost = 1125,
                        descLoca = "When an item disappears from your inventory, a coin may appear in this slot",
                        icon = "Bank10"
                    }
                }
            });
            Add(new StandardUpgrade()
            {
                id = "Glue",
                weight = 75,
                levels = new UpgradeLevel[4]
                {
                    new UpgradeLevel()
                    {
                        cost = 350,
                        descLoca = "Increase glue stick time",
                        icon = "Glue1"
                    },
                    new UpgradeLevel()
                    {
                        cost = 550,
                        descLoca = "Increase glue stick time",
                        icon = "Glue2"
                    },
                    new UpgradeLevel()
                    {
                        cost = 750,
                        descLoca = "Increase glue stick time",
                        icon = "Glue3"
                    },
                    new UpgradeLevel()
                    {
                        cost = 1500,
                        descLoca = "Increase glue stick time",
                        icon = "Glue4"
                    }
                }
            });
        }
    }
}
