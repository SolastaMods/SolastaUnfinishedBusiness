def unpack_record(record):
    term = ""
    text = ""
    try:
        (term, text) = record.split("\t", 1)
        text = text.strip()
    except:
        term = record

    return term, record


def get_records(filename):
    try:
        with open(filename, "rt", encoding="utf-8") as f:
            record = "\n"
            while record:
                record = f.readline()
                if record: yield unpack_record(record)
            print()

    except FileNotFoundError:
        print("ERROR")

rules = [

    {
        "group": "Tinkerer-en.txt",
        "matches": [
            "Tinkerer",
            "ResistantArmor",
            "Artificer", 
            "Artillerist",
            "Artillery",
            "Artificer", 
            "Infusion",
            "BattleSmith",
            "Scout",
            "Sentinel",
            "PowerInfuse",
            "PowerTransmute", 
            "PowerInfuseArmor",
            "SummonArtillery",
            "FlameArtillery",
            "ForceArtillery",
            "SummonProtector",
            "ResummonArtilleryConstruct",
            "TempHP",
            "Elixir",
            "Alchemical",
            "ThunderStruck",
            "LightningSpear",
            "SelfDestruction",
            "HalfCoverShield",
            "ArcaneFirearm"
        ],
        "lines": []
    },
    {
        "group": "PathOfTheLight-en.txt",
        "matches": ["PathOfTheLight"],
        "lines": []
    },
    {
        "group": "UrPriest-en.txt",
        "matches": ["UrPriest", "&Domain", "HalfLifeCondition"],
        "lines": []
    },
    {
        "group": "Bolgrif-en.txt",
        "matches": ["Bolgrif", "Abyssal"],
        "lines": []
    },
    {
        "group": "Gnome-en.txt",
        "matches": ["Gnome", "Gnomish"],
        "lines": []
    },
    {
        "group": "Monk-en.txt",
        "matches": [
            "Monk",
            "DiamondSoul",
            "PathOf"],
        "lines": []
    },
    {
        "group": "ConArtist-en.txt",
        "matches": ["ConArtist"],
        "lines": []
    },
    {
        "group": "SpellMaster-en.txt",
        "matches": ["SpellMaster"],
        "lines": []
    },
    {
        "group": "ArcaneFighter-en.txt",
        "matches": ["ArcaneFighter"],
        "lines": []
    },
    {
        "group": "SpellShield-en.txt",
        "matches": ["SpellShield", "MeleeWizard"],
        "lines": []
    },
    {
        "group": "LifeTransmuter-en.txt",
        "matches": ["LifeTransmuter", "Transmute"],
        "lines": []
    },
    {
        "group": "MasterManipulator-en.txt",
        "matches": ["Manipulator"],
        "lines": []
    },
    {
        "group": "Opportunist-en.txt",
        "matches": ["Opportunist"],
        "lines": []
    },
    {
        "group": "Arcanist-en.txt",
        "matches": ["Arcanist", "&Arcane"],
        "lines": []
    },
    {
        "group": "Tactician-en.txt",
        "matches": [
            "Tactician",
            "Tactition",
            "InspirePower",
            "CounterStrike",
            "KnockDown",
            "Gambit"],
        "lines": []
    },
    {
        "group": "Warlock-en.txt",
        "matches": [
            "Warlock",
            "Patron",
            "Pact",
            "Moonlit",
            "MoonLit",
            "AncientForest",
            "OneWithShadows",
            "EldritchBlast",
            "Eldritch",
            "ElementalPatron",
            "ElementalPact",
            "AHSoulBlade",
            "Rift",
            "Chain",
            "Blast",
            "Moon",
            "ThiefofFive",
            "DH_ElementalFormPool",
            "Giftofthe",
            "BeguilingInfluence",
            "BondoftheTalisman",
            "DevilsSight",
            "Blink",
            "FadeIntoTheVoid",
            "EyesoftheRuneKeeper",
            "HerbalBrew",
            "Otherworldly",
            "SoulBlade",
            "ShroudofShadow",
            "ThirstingBlade",
            "WallofThorns"
        ],
        "lines": []
    },
    {
        "group": "Marshal-en.txt",
        "matches": ["Marshal"],
        "lines": []
    },
    {
        "group": "RoyalKnight-en.txt",
        "matches": [
            "Royal", 
            "Rallying",
            "InspiringSurge"],
        "lines": []
    },
    {
        "group": "DruidForestGuardian-en.txt",
        "matches": ["DruidForestGuardian"],
        "lines": []
    },
    {
        "group": "Witch-en.txt",
        "matches": [
            "Witch",
            "Coven", 
            "Abate", 
            "Disorient", 
            "Charm", 
            "EvilEye", 
            "Frenzied", 
            "Pox",
            "Debilitated",
            "Apathy",
            "Ruin"],
        "lines": []
    },
    {
        "group": "ToadKing-en.txt",
        "matches": [
            "ToadKing", 
            "Toad"],
        "lines": []
    },
    {
        "group": "Equipment-en.txt",
        "matches": [
            "Equipment/&",
            "Item/&Crafting"],
        "lines": []
    },
    {
        "group": "Spell-en.txt",
        "matches": ["Spell"],
        "lines": []
    },
    {
        "group": "Feat-en.txt",
        "matches": ["Feat/&"],
        "lines": []
    },
    {
        "group": "FlexibleBackgrounds-en.txt",
        "matches": ["FlexibleBackgrounds/&"],
        "lines": []
    },
    {
        "group": "FlexibleRaces-en.txt",
        "matches": [
            "FlexibleRaces/&",
            "Race/&"],
        "lines": []
    },
    {
        "group": "CraftyFeats-en.txt",
        "matches": [
            "CraftyFeats"
        ],
        "lines": []
    },
    {
        "group": "FightingStyle-en.txt",
        "matches": [
            "BlindFighting",
            "Crippling",
            "Pugilist",
            "TitanFighting"
        ],
        "lines": []
    },
    {
        "group": "Items-en.txt",
        "matches": ["Item/&"],
        "lines": []
    },
    {
        "group": "Remaining-en.txt",
        "matches": ["&"],
        "lines": []
    },
]

for term, line in get_records("Translations-en.txt"):
    found = False
    for rule in rules:
            for match in rule["matches"]:
                if match in term:
                    if not found: 
                        rule["lines"].append(line)
                        found = True
        

for rule in rules:
    with open(rule["group"], "wt", encoding="utf-8") as f:
        for line in rule["lines"]:
            f.write(line) 