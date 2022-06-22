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
        "group": "Tinkerer-fr.txt",
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
        "group": "Warlock-fr.txt",
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
        "group": "Witch-fr.txt",
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
        "group": "Monk-fr.txt",
        "matches": [
            "Monk",
            "DiamondSoul",
            "PathOf"],
        "lines": []
    },

    # RACES

    {
        "group": "Bolgrif-fr.txt",
        "matches": ["Bolgrif", "Abyssal"],
        "lines": []
    },
    {
        "group": "Gnome-fr.txt",
        "matches": ["Gnome", "Gnomish"],
        "lines": []
    },

    # SUBCLASSES

    {
        "group": "PathOfTheLight-fr.txt",
        "matches": [
            "PathOfTheLight"
        ],
        "lines": []
    },
    {
        "group": "UrPriest-fr.txt",
        "matches": [
            "UrPriest",
            "&Domain",
            "HalfLifeCondition"
        ],
        "lines": []
    },
    {
        "group": "ConArtist-fr.txt",
        "matches": [
            "ConArtist"
        ],
        "lines": []
    },
    {
        "group": "SpellMaster-fr.txt",
        "matches": [
            "SpellMaster"
        ],
        "lines": []
    },
    {
        "group": "ArcaneFighter-fr.txt",
        "matches": [
            "ArcaneFighter"
        ],
        "lines": []
    },
    {
        "group": "SpellShield-fr.txt",
        "matches": [
            "SpellShield",
            "MeleeWizard"
        ],
        "lines": []
    },
    {
        "group": "LifeTransmuter-fr.txt",
        "matches": [
            "LifeTransmuter", 
            "Transmute"
        ],
        "lines": []
    },
    {
        "group": "MasterManipulator-fr.txt",
        "matches": [
            "Manipulator"
        ],
        "lines": []
    },
    {
        "group": "Opportunist-fr.txt",
        "matches": [
            "Opportunist"
        ],
        "lines": []
    },
    {
        "group": "Arcanist-fr.txt",
        "matches": [
            "Arcanist",
            "&Arcane"
        ],
        "lines": []
    },
    {
        "group": "Tactician-fr.txt",
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
        "group": "Marshal-fr.txt",
        "matches": [
            "Marshal",
            "CoordinatedAttack"
        ],
        "lines": []
    },
    {
        "group": "RoyalKnight-fr.txt",
        "matches": [
            "Royal", 
            "Rallying",
            "InspiringSurge"
        ],
        "lines": []
    },
    {
        "group": "DruidForestGuardian-fr.txt",
        "matches": [
            "DruidForestGuardian",
            "BarkWard"
        ],
        "lines": []
    },
    {
        "group": "ToadKing-fr.txt",
        "matches": [
            "ToadKing", 
            "Toad"
        ],
        "lines": []
    },

    # SPELLS
    {
        "group": "Spell-fr.txt",
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
        "group": "ItemsCrafting-fr.txt",
        "matches": [
            "Equipment/&",
            "Polearm",
            "HandwrapsOfPulling",

            "Item/&"
        ],
        "lines": []
    },
    {
        "group": "Feat-fr.txt",
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
        "group": "FlexibleBackgrounds-fr.txt",
        "matches": [
            "FlexibleBackgrounds/&"
        ],
        "lines": []
    },
    {
        "group": "FlexibleRaces-fr.txt",
        "matches": [
            "FlexibleRaces/&",
            "Race/&"
        ],
        "lines": []
    },
    {
        "group": "FightingStyle-fr.txt",
        "matches": [
            "BlindFighting",
            "Crippling",
            "Pugilist",
            "TitanFighting"
        ],
        "lines": []
    },
    {
        "group": "QualityOfLife-fr.txt",
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
        "group": "Level20-fr.txt",
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
        "group": "Remaining-fr.txt",
        "matches": [
            "&"
        ],
        "lines": []
    },
]

for term, line in get_records("Translations-fr.txt"):
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