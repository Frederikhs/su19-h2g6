using System;
using System.Collections.Generic;
using System.IO;

namespace SpaceTaxi_1 {
    public class TextLoader {

        private string[] allLevelText;

        // A TextLoader loads an entire .txt file into a string array 
        public TextLoader(string levelString) {
            var path = "../../Levels/"+levelString+".txt";
            allLevelText = File.ReadAllLines(path);
        }

        //Method for getting a level map
        public List<string> get_lvl_struc_string() {
            List<string> map = new List<string>(); 
            for (int i = 0; i < 23; i++) {
                map.Add(allLevelText[i]);
            }

            return map;
        }

        //Method for getting level info
        public List<string> get_lvl_info() {
            List<string> levelInfo = new List<string>();
            levelInfo.Add(allLevelText[24]);
            levelInfo.Add(allLevelText[25]);
            
            return levelInfo;
        }
        
        //Method for getting level legends
        public List<string> get_lvl_legends() {
            List<string> levelLegends = new List<string>();

            foreach (var line in allLevelText) {
                if (line.Length >= 2) {
                    if (line[1] == ')') {
                        levelLegends.Add(line);
                    }
                }
            }
            
            return levelLegends;
        }
        
        //Method for getting customer info
        public List<string> get_customer_info() {
            List<string> custInfo = new List<string>();
            
            foreach (var line in allLevelText) {
                if (line.Contains("Customer")) {
                    custInfo.Add(line);
                }
            }
            
            return custInfo;
        }
        
        
    }
}