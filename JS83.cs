using UnityEngine;
using GrindFest; // API-Namespace, inkl. AutomaticHero

namespace JS83BotNamespace
{
    // Konfigurationsklasse mit globalen Einstellungen
    public static class Config
    {
        public static int HealthThreshold = 40;          // Unter diesem Wert wird ein Gesundheitstrank getrunken
        public static int CriticalHealthThreshold = 20;    // Kritischer Wert: unter diesem Wert weglaufen
        public static float AttackRange = 10.0f;           // Reichweite für Angriffe
        public static float FollowRange = 5.0f;            // Reichweite, um dem Cursor zu folgen
        public static float RunAwayDistance = 15.0f;       // Distanz, die beim Wegrennen zurückgelegt werden soll
        public static float LootRange = 5.0f;              // Reichweite, in der nach Items gesucht wird

        public static void LoadConfig()
        {
            Debug.Log("Konfiguration geladen.");
        }
    }

    // Modul zur Verwaltung von Partymitgliedern (Stub – erweiterbar)
    public class AutoParty
    {
        public AutoParty()
        {
            Debug.Log("AutoParty-Modul initialisiert.");
        }

        public void HealParty()
        {
            // Beispiel: Überprüfe und heile Party-Mitglieder
            Debug.Log("Party-Heilroutine (Stub).");
        }
    }

    // Modul zur Verwaltung des Inventars (TaschenVerwaltung)
    public class TaschenVerwaltung
    {
        private AutomaticHero hero;

        public TaschenVerwaltung(AutomaticHero hero)
        {
            this.hero = hero;
            Debug.Log("TaschenVerwaltung initialisiert.");
        }

        public void ManageInventory()
        {
            // Beispiel: Suche nach bestimmten Items im Inventar
            var item = hero.FindItemInInventory("Potion", false);
            if (item != null)
            {
                Debug.Log("Potion im Inventar gefunden.");
            }
        }
    }

    // Modul zur Verwaltung von NPCs, Questgebern und Händlern (Stub – erweiterbar)
    public class NPCVerwaltung
    {
        public NPCVerwaltung()
        {
            Debug.Log("NPCVerwaltung initialisiert.");
        }

        public void InteractWithNPC(string npcName)
        {
            // Hier könntest du eine Interaktion mit NPCs implementieren.
            Debug.Log($"Interaktion mit NPC {npcName} gestartet.");
        }
    }

    // Hauptklasse des Bots – der Klassenname MUSS mit dem Heldennamen "JS83" übereinstimmen.
    public class JS83 : AutomaticHero
    {
        private AutoParty autoParty;
        private TaschenVerwaltung taschenVerwaltung;
        private NPCVerwaltung npcVerwaltung;

        // Flag, ob der Bot pausiert ist
        private bool paused = false;

        // Wird einmalig beim Start des GameObjects aufgerufen
        void Start()
        {
            Config.LoadConfig();

            // Module initialisieren
            autoParty = new AutoParty();
            taschenVerwaltung = new TaschenVerwaltung(this);
            npcVerwaltung = new NPCVerwaltung();

            Debug.Log("JS83 Bot gestartet – bereit für den Einsatz.");
        }

        // Wird in jedem Frame aufgerufen
        void Update()
        {
            // Tasten abfragen:
            // F4: Pause ein/aus schalten
            if (Input.GetKeyDown(KeyCode.F4))
            {
                paused = !paused;
                Debug.Log("Bot " + (paused ? "paused" : "resumed"));
            }
            
            // F3: Inventar öffnen
            if (Input.GetKeyDown(KeyCode.F3))
            {
                OpenInventory();
                Debug.Log("Inventar geöffnet.");
            }
            
            // Falls der Bot pausiert ist, keine weiteren Aktionen ausführen.
           
            if (paused) return;
            
            // 1. Kritische Gesundheit: Wenn der Wert unter CriticalHealthThreshold liegt, weglaufen.
            if (Health < Config.CriticalHealthThreshold)
            {
                RunAwayFromNearestEnemy(Config.RunAwayDistance);
                DrinkHealthPotion();
                Debug.Log("Leben kritisch – weglaufen!");
                return;
            }
            
            // 2. Niedrige Gesundheit: Falls unter HealthThreshold, trinke einen Gesundheitstrank.
            if (Health < Config.HealthThreshold)
            {
                DrinkHealthPotion();
                Debug.Log("Gesundheit niedrig – trinke Gesundheitstrank.");
                return;
            }

            // 3. Gegner in Reichweite: Greife den nächsten Gegner an.
            if (FindNearestEnemy(Config.AttackRange, 0.0f) != null)
            {
                AttackNearestEnemy(Config.AttackRange);
                Debug.Log("Gegner in Reichweite – Angriff ausgeführt.");
                return;
            }

            // 4. Loot: Überprüfe, ob sich Items in der Nähe befinden, und hebe das erste gefundene auf.
            var lootItem = FindNearestItemOnGround("", "", "", Config.LootRange);
            if (lootItem != null)
            {
                Debug.Log("Loot gefunden – Item wird aufgesammelt.");
                PickUp(lootItem);
                return;
            }

            // 5. Standardaktion: Folge dem Cursor und greife Gegner an.
            FollowCursorAndAttack();
        }
    }
}
