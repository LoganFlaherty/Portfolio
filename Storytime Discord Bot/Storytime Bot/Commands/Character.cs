using System.Collections.Generic;

namespace Storytime_Bot.Commands
{
    public class Character
    {
        //Properties
        public string Name { get; set; }
        public double Expertise { get; set; }
        public double ExpertiseReserve { get; set; }
        public double ExpertisePrestige { get; set; }
        public double Faith { get; set; }
        public double FaithReserve { get; set; }
        public double FaithPrestige { get; set; }
        public double Influence { get; set; }
        public double InfluenceReserve { get; set; }
        public double InfluencePrestige { get; set; }
        public double Perception { get; set; }
        public double PerceptionReserve { get; set; }
        public double PerceptionPrestige { get; set; }
        public double Physique { get; set; }
        public double PhysiqueReserve { get; set; }
        public double PhysiquePrestige { get; set; }
        public List<string> Tags { get; set; }

        //Constructor
        public Character(string name, double expertise, double expertisePrestige, double faith, double faithPrestige, 
            double influence, double influencePrestige, double perception, double perceptionPrestige, double physique, double physiquePrestige, List<string> tags)
        {
            Name = name;
            Expertise = expertise;
            ExpertiseReserve = expertise;
            ExpertisePrestige = expertisePrestige;
            Faith = faith;
            FaithReserve = faith;
            FaithPrestige = faithPrestige;
            Influence = influence;
            InfluenceReserve = influence;
            InfluencePrestige = influencePrestige;
            Perception = perception;
            PerceptionReserve = perception;
            PerceptionPrestige = perceptionPrestige;
            Physique = physique;
            PhysiqueReserve = physique;
            PhysiquePrestige = physiquePrestige;
            Tags = tags;
        }

        public Character(string name, double expertise, double expertiseReserve, double expertisePrestige, double faith, double faithReserve, double faithPrestige,
            double influence, double influenceReserve, double influencePrestige, double perception, double perceptionReserve, double perceptionPrestige,
            double physique, double physiqueReserve, double physiquePrestige, List<string> tags)
        {
            Name = name;
            Expertise = expertise;
            ExpertiseReserve = expertiseReserve;
            ExpertisePrestige = expertisePrestige;
            Faith = faith;
            FaithReserve = faithReserve;
            FaithPrestige = faithPrestige;
            Influence = influence;
            InfluenceReserve = influenceReserve;
            InfluencePrestige = influencePrestige;
            Perception = perception;
            PerceptionReserve = perceptionReserve;
            PerceptionPrestige = perceptionPrestige;
            Physique = physique;
            PhysiqueReserve = physiqueReserve;
            PhysiquePrestige = physiquePrestige;
            Tags = tags;
        }

        //Methods
        public string ListTags()
        {
            string str = "Tags:\n";
            foreach (string tag in Tags)
            {
                str += $"- {tag}\n";
            }
            return str;
        }
        
        public void AddTag(string tag)
        {
            Tags.Add(tag);
        }

        public void RemoveTag(string tag)
        {
            Tags.Remove(tag);
        }

        public override string ToString()
        {
            string str = string.Empty;
            str += $"Name: {Name}\n";
            str += "Attributes: Prowess/Reserve/Prestige\n";
            str += $"Expertise: {Expertise}/{ExpertiseReserve}/{ExpertisePrestige}\n";
            str += $"Faith: {Faith}/{FaithReserve}/{FaithPrestige}\n";
            str += $"Influence: {Influence}/{InfluenceReserve}/{InfluencePrestige}\n";
            str += $"Perception: {Perception}/{PerceptionReserve}/{PerceptionPrestige}\n";
            str += $"Physique: {Physique}/{PhysiqueReserve}/{PhysiquePrestige}\n";
            str += ListTags();
            return str;
        }

        public string FileToString()
        {
            string str = string.Empty;
            str += $"{Name},{Expertise},{ExpertiseReserve},{ExpertisePrestige},{Faith}," +
                $"{FaithReserve},{FaithPrestige},{Influence},{InfluenceReserve},{InfluencePrestige}," +
                $"{Perception},{PerceptionReserve},{PerceptionPrestige},{Physique},{PhysiqueReserve},{PhysiquePrestige}";
            foreach (string tag in Tags)
            {
                str += $",{tag}";
            }
            return str;
        }
    }
}
