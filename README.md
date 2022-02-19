# MomodoraRandomizer
 A LiveSplit component that is a randomizer for Momodora: Reverie Under the Moonlight. Currently supporting game versions v1.05b and v1.07
 
 ## Installation
 First, make sure that you have a LiveSplit installation http://livesplit.org/downloads/.
 
 1. Download the "MomodoraRandomizer.dll" from releases. https://github.com/axelkarlsson/MomodoraRandomizer/releases
 2. Navigate to your LiveSplit install, and put "MomodoraRandomizer.dll" in <LiveSplitDir>/Components
 3. Launch LiveSplit and Edit Layout, add the Momodora Randomizer component from the "Other" category
![image](https://user-images.githubusercontent.com/26115597/154794577-4dd8d8fb-a589-4a48-b257-a73940f76956.png)

 ## How to use
 It is inteded to start the randomizer from a fresh launch of the game on a fresh savefile. Starting a randomizer in other circumstances may lead to instabilities/crashes.
 
To start a randomizer, simply start the timer. This should be done when you have started a new file and the Momodora: Reverie Under the Moonlight autosplitter works well for this. Once a run has started, a text will appear on the component in LiveSplit saying "1.05b/1.07 randomizer ready to go!" if it correctly detected the game.
 
 During play picking up an item will only display the original item that you were supposed to get, you have to look at the randomizer component to see the display of what item you actually found.
 
 Whenever an active item is picked up it won't be useable until you save at a bell.
 
 ## Settings
 ![image](https://user-images.githubusercontent.com/26115597/154794956-cbdb9425-c023-44cd-9604-e1e5614f6d84.png)
 You can either use a set seed, or get a random seed.
In the settings you can toggle whether Ivory Bugs and Vitality Fragments are included in the randomization process or not, and enabling Hard Mode allows bosses to drop key items and assumes that either Garden Key or Bakman Patch is enough to get into Whiteleaf Memorial Park.
 Shop items and rewards from NPC's (giving the Dirty Shroom to the imp, Ivory Bugs to the bunny and Hazel Badge to Eri) are all randomized.
 Arsonist boss item is not randomized.
 
 If any issues are detected you can contact me on discord DreamFox#2831
