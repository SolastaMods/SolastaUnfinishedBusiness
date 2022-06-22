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
        "group": "Tinkerer-es.txt",
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
        "group": "Warlock-es.txt",
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
        "group": "Witch-es.txt",
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
        "group": "Monk-es.txt",
        "matches": [
            "Monk",
            "DiamondSoul",
            "PathOf"],
        "lines": []
    },

    # RACES

    {
        "group": "Bolgrif-es.txt",
        "matches": ["Bolgrif", "Abyssal"],
        "lines": []
    },
    {
        "group": "Gnome-es.txt",
        "matches": ["Gnome", "Gnomish"],
        "lines": []
    },

    # SUBCLASSES

    {
        "group": "PathOfTheLight-es.txt",
        "matches": [
            "PathOfTheLight"
        ],
        "lines": []
    },
    {
        "group": "UrPriest-es.txt",
        "matches": [
            "UrPriest",
            "&Domain",
            "HalfLifeCondition"
        ],
        "lines": []
    },
    {
        "group": "ConArtist-es.txt",
        "matches": [
            "ConArtist"
        ],
        "lines": []
    },
    {
        "group": "SpellMaster-es.txt",
        "matches": [
            "SpellMaster"
        ],
        "lines": []
    },
    {
        "group": "ArcaneFighter-es.txt",
        "matches": [
            "ArcaneFighter"
        ],
        "lines": []
    },
    {
        "group": "SpellShield-es.txt",
        "matches": [
            "SpellShield",
            "MeleeWizard"
        ],
        "lines": []
    },
    {
        "group": "LifeTransmuter-es.txt",
        "matches": [
            "LifeTransmuter", 
            "Transmute"
        ],
        "lines": []
    },
    {
        "group": "MasterManipulator-es.txt",
        "matches": [
            "Manipulator"
        ],
        "lines": []
    },
    {
        "group": "Opportunist-es.txt",
        "matches": [
            "Opportunist"
        ],
        "lines": []
    },
    {
        "group": "Arcanist-es.txt",
        "matches": [
            "Arcanist",
            "&Arcane"
        ],
        "lines": []
    },
    {
        "group": "Tactician-es.txt",
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
        "group": "Marshal-es.txt",
        "matches": [
            "Marshal",
            "CoordinatedAttack"
        ],
        "lines": []
    },
    {
        "group": "RoyalKnight-es.txt",
        "matches": [
            "Royal", 
            "Rallying",
            "InspiringSurge"
        ],
        "lines": []
    },
    {
        "group": "DruidForestGuardian-es.txt",
        "matches": [
            "DruidForestGuardian",
            "BarkWard"
        ],
        "lines": []
    },
    {
        "group": "ToadKing-es.txt",
        "matches": [
            "ToadKing", 
            "Toad"
        ],
        "lines": []
    },

    # SPELLS
    {
        "group": "Spell-es.txt",
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
        "group": "ItemsCrafting-es.txt",
        "matches": [
            "Equipment/&",
            "Polearm",
            "HandwrapsOfPulling",

            "Item/&"
        ],
        "lines": []
    },
    {
        "group": "Feat-es.txt",
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
        "group": "FlexibleBackgrounds-es.txt",
        "matches": [
            "FlexibleBackgrounds/&"
        ],
        "lines": []
    },
    {
        "group": "FlexibleRaces-es.txt",
        "matches": [
            "FlexibleRaces/&",
            "Race/&"
        ],
        "lines": []
    },
    {
        "group": "FightingStyle-es.txt",
        "matches": [
            "BlindFighting",
            "Crippling",
            "Pugilist",
            "TitanFighting"
        ],
        "lines": []
    },
    {
        "group": "QualityOfLife-es.txt",
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
        "group": "Level20-es.txt",
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
        "group": "Remaining-es.txt",
        "matches": [
            "&"
        ],
        "lines": []
    },
]

for term, line in get_records("Translations-es.txt"):
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