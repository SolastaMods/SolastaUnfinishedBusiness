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

    # CLASSES

    {
        "group": "Tinkerer-zh-CN.txt",
        "matches": [
            "Tinkerer",
            "ResistantArmor",
            "Artificer", 
            "Artillerist",
            "DisadvantageOnAttackByEnemy",
            "AdvantageAttackOnEnemy",
            "Artillery",
            "Artificer", 
            "Infusion",
            "BattleSmith",
            "Scout",
            "Sentinel",
            "AdditionalDamageUpgradedConstruct",
            "PowerInfuse",
            "BlindingWeapon",
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
            "ArcaneFirearm",
            "ConditionResistnat",
            "Specialist",
            "ExtraDamageOnAttack"
        ],
        "lines": []
    },
    {
        "group": "Warlock-zh-CN.txt",
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
            "Trickster",
            "BeguilingInfluence",
            "BondoftheTalisman",
            "DevilsSight",
            "Blink",
            "FadeIntoTheVoid",
            "EyesoftheRuneKeeper",
            "HerbalBrew",
            "SoulEmpowered",
            "DreadfulWord",
            "CounterFormDismissCreature",
            "Otherworldly",
            "SoulBlade",
            "ShroudofShadow",
            "ThirstingBlade",
            "AdditionalDamageElementalDamage",
            "WallofThorns",
            "FindFamiliarBundlePower",
            "ArmorofShadows",
            "AscendantStep",
            "DHConjureMinorElementals",
            "FiendishVigor",
            "MiretheMind",
            "DHWardingBond"
        ],
        "lines": []
    },
    {
        "group": "Witch-zh-CN.txt",
        "matches": [
            "Witch",
            "Malediction",
            "Coven", 
            "Abate", 
            "Disorient", 
            "Charm", 
            "EvilEye", 
            "Frenzied", 
            "Pox",
            "Debilitated",
            "Apathy",
            "WitchOwlFamiliar",
            "Ruin"],
        "lines": []
    },
    {
        "group": "Monk-zh-CN.txt",
        "matches": [
            "Monk",
            "DiamondSoul",
            "PathOf"],
        "lines": []
    },

    # RACES

    {
        "group": "Bolgrif-zh-CN.txt",
        "matches": ["Bolgrif", "Abyssal"],
        "lines": []
    },
    {
        "group": "Gnome-zh-CN.txt",
        "matches": ["Gnome", "Gnomish"],
        "lines": []
    },

    # SUBCLASSES

    {
        "group": "PathOfTheLight-zh-CN.txt",
        "matches": [
            "PathOfTheLight"
        ],
        "lines": []
    },
    {
        "group": "UrPriest-zh-CN.txt",
        "matches": [
            "UrPriest",
            "&Domain",
            "HalfLifeCondition"
        ],
        "lines": []
    },
    {
        "group": "ConArtist-zh-CN.txt",
        "matches": [
            "ConArtist"
        ],
        "lines": []
    },
    {
        "group": "SpellMaster-zh-CN.txt",
        "matches": [
            "SpellMaster"
        ],
        "lines": []
    },
    {
        "group": "ArcaneFighter-zh-CN.txt",
        "matches": [
            "ArcaneFighter"
        ],
        "lines": []
    },
    {
        "group": "SpellShield-zh-CN.txt",
        "matches": [
            "SpellShield",
            "MeleeWizard"
        ],
        "lines": []
    },
    {
        "group": "LifeTransmuter-zh-CN.txt",
        "matches": [
            "LifeTransmuter", 
            "Transmute"
        ],
        "lines": []
    },
    {
        "group": "MasterManipulator-zh-CN.txt",
        "matches": [
            "Manipulator"
        ],
        "lines": []
    },
    {
        "group": "Opportunist-zh-CN.txt",
        "matches": [
            "Opportunist"
        ],
        "lines": []
    },
    {
        "group": "Arcanist-zh-CN.txt",
        "matches": [
            "Arcanist",
            "&Arcane"
        ],
        "lines": []
    },
    {
        "group": "Tactician-zh-CN.txt",
        "matches": [
            "Tactician",
            "Tactition",
            "InspirePower",
            "CounterStrike",
            "KnockDown",
            "Gambit"
        ],
        "lines": []
    },
    {
        "group": "Marshal-zh-CN.txt",
        "matches": [
            "Marshal",
            "CoordinatedAttack"
        ],
        "lines": []
    },
    {
        "group": "RoyalKnight-zh-CN.txt",
        "matches": [
            "Royal", 
            "Rallying",
            "InspiringSurge"
        ],
        "lines": []
    },
    {
        "group": "DruidForestGuardian-zh-CN.txt",
        "matches": [
            "DruidForestGuardian",
            "BarkWard"
        ],
        "lines": []
    },
    {
        "group": "ToadKing-zh-CN.txt",
        "matches": [
            "ToadKing", 
            "Toad"
        ],
        "lines": []
    },

    # SPELLS
    {
        "group": "Spell-zh-CN.txt",
        "matches": [
            "Spell",
            "CouatlBite",
            "SunlightBlade",
            "ResonatingStrike",
            "Mule",
            "DHHolyAuraBlinding",
            "CustomCouat",
            "OwlFamiliar"
        ],
        "lines": []
    },

    # OTHERS

    {
        "group": "ItemsCrafting-zh-CN.txt",
        "matches": [
            "Equipment/&",
            "Polearm",
            "HandwrapsOfPulling",

            "Item/&"
        ],
        "lines": []
    },
    {
        "group": "Feat-zh-CN.txt",
        "matches": [
            "Feat/&", 
            "PowerAttack",
            "Shady",
            "Deadeye",
            "DualFlurry",
            "Torchbearer",
            "Rage",
            "Warcaster",
            "FeatPrerequisite",
            "CraftyFeats",
            "HelpAction"
        ],
        "lines": []
    },
    {
        "group": "FlexibleBackgrounds-zh-CN.txt",
        "matches": [
            "FlexibleBackgrounds/&"
        ],
        "lines": []
    },
    {
        "group": "FlexibleRaces-zh-CN.txt",
        "matches": [
            "FlexibleRaces/&",
            "Race/&"
        ],
        "lines": []
    },
    {
        "group": "FightingStyle-zh-CN.txt",
        "matches": [
            "BlindFighting",
            "Crippling",
            "Pugilist",
            "TitanFighting"
        ],
        "lines": []
    },
    {
        "group": "QualityOfLife-zh-CN.txt",
        "matches": [
            "/&ZSRespec",
            "ContentPack/&",
            "Message/&",
            "UI/&",
            "Tooltip/&",
            "ToolTip/&",
            "Requirement/&",
            "Stage/&",
            "Screen/&",
            "Reaction/&",
            "ExportPdf",
            "LevelAndExperience",
            "BardSkills",
            "AlwaysBeard",
            "EmptyString",
            "FailureFlagTarget",
            "GadgetParametersCustomRemove",
            "FlatRoomsTitle"
        ],
        "lines": []
    },
    {
        "group": "Level20-zh-CN.txt",
        "matches": [
            "RangerVanish",
            "RogueSlippery",
            "PrimalChampion",
            "BlindSense",
            "/&ZS",
            "IndomitableMight",
            "IndomitableResistance",
            "PaladinAuraOf",
            "FeralSenses"
        ],
        "lines": []
    },
    {
        "group": "Remaining-zh-CN.txt",
        "matches": [
            "&"
        ],
        "lines": []
    },
]

for term, line in get_records("Translations-zh-CN.txt"):
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