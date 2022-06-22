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
        "group": "Tinkerer-pt-BR.txt",
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
        "group": "Warlock-pt-BR.txt",
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
        "group": "Witch-pt-BR.txt",
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
        "group": "Monk-pt-BR.txt",
        "matches": [
            "Monk",
            "DiamondSoul",
            "PathOf"],
        "lines": []
    },

    # RACES

    {
        "group": "Bolgrif-pt-BR.txt",
        "matches": ["Bolgrif", "Abyssal"],
        "lines": []
    },
    {
        "group": "Gnome-pt-BR.txt",
        "matches": ["Gnome", "Gnomish"],
        "lines": []
    },

    # SUBCLASSES

    {
        "group": "PathOfTheLight-pt-BR.txt",
        "matches": [
            "PathOfTheLight"
        ],
        "lines": []
    },
    {
        "group": "UrPriest-pt-BR.txt",
        "matches": [
            "UrPriest",
            "&Domain",
            "HalfLifeCondition"
        ],
        "lines": []
    },
    {
        "group": "ConArtist-pt-BR.txt",
        "matches": [
            "ConArtist"
        ],
        "lines": []
    },
    {
        "group": "SpellMaster-pt-BR.txt",
        "matches": [
            "SpellMaster"
        ],
        "lines": []
    },
    {
        "group": "ArcaneFighter-pt-BR.txt",
        "matches": [
            "ArcaneFighter"
        ],
        "lines": []
    },
    {
        "group": "SpellShield-pt-BR.txt",
        "matches": [
            "SpellShield",
            "MeleeWizard"
        ],
        "lines": []
    },
    {
        "group": "LifeTransmuter-pt-BR.txt",
        "matches": [
            "LifeTransmuter", 
            "Transmute"
        ],
        "lines": []
    },
    {
        "group": "MasterManipulator-pt-BR.txt",
        "matches": [
            "Manipulator"
        ],
        "lines": []
    },
    {
        "group": "Opportunist-pt-BR.txt",
        "matches": [
            "Opportunist"
        ],
        "lines": []
    },
    {
        "group": "Arcanist-pt-BR.txt",
        "matches": [
            "Arcanist",
            "&Arcane"
        ],
        "lines": []
    },
    {
        "group": "Tactician-pt-BR.txt",
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
        "group": "Marshal-pt-BR.txt",
        "matches": [
            "Marshal",
            "CoordinatedAttack"
        ],
        "lines": []
    },
    {
        "group": "RoyalKnight-pt-BR.txt",
        "matches": [
            "Royal", 
            "Rallying",
            "InspiringSurge"
        ],
        "lines": []
    },
    {
        "group": "DruidForestGuardian-pt-BR.txt",
        "matches": [
            "DruidForestGuardian",
            "BarkWard"
        ],
        "lines": []
    },
    {
        "group": "ToadKing-pt-BR.txt",
        "matches": [
            "ToadKing", 
            "Toad"
        ],
        "lines": []
    },

    # SPELLS
    {
        "group": "Spell-pt-BR.txt",
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
        "group": "ItemsCrafting-pt-BR.txt",
        "matches": [
            "Equipment/&",
            "Polearm",
            "HandwrapsOfPulling",

            "Item/&"
        ],
        "lines": []
    },
    {
        "group": "Feat-pt-BR.txt",
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
        "group": "FlexibleBackgrounds-pt-BR.txt",
        "matches": [
            "FlexibleBackgrounds/&"
        ],
        "lines": []
    },
    {
        "group": "FlexibleRaces-pt-BR.txt",
        "matches": [
            "FlexibleRaces/&",
            "Race/&"
        ],
        "lines": []
    },
    {
        "group": "FightingStyle-pt-BR.txt",
        "matches": [
            "BlindFighting",
            "Crippling",
            "Pugilist",
            "TitanFighting"
        ],
        "lines": []
    },
    {
        "group": "QualityOfLife-pt-BR.txt",
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
        "group": "Level20-pt-BR.txt",
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
        "group": "Remaining-pt-BR.txt",
        "matches": [
            "&"
        ],
        "lines": []
    },
]

for term, line in get_records("Translations-pt-BR.txt"):
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