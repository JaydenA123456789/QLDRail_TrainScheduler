# **QLDRail Train Scheduler**

## **Project Overview**  
QLDRail Train Scheduler is a **C# console application** that processes train route data from a text file and generates a description of the train’s route based on stopping patterns.

## **File Structure**
To add files to be run, add them to the QLDRail_TrainScheduler/TrainSequenceFiles, I have left some example files in TrainSequenceFiles that cover a number of cases.

## **Example Usage**
1.  Add valid scenario to QLDRail_TrainScheduler/TrainSequenceFiles
2.  Navigate to the file directory of the Project (C:\[Your file location]\QLDRail_TrainScheduler\QLDRail_TrainScheduler)
3.  Run the application with: dotnet run "[YourFile].txt"

For the text file:
"1, True
2, False
3, True
4, False
5, True
6, True"

The output will be: "This train runs express from 1 to 5, stopping only at 3 then stops at all stations except 7"

## **Edgecase Assumptions**
When handling complex stop sequences, the program divides them into sections. However, certain sequences may have multiple valid interpretations.

For example:
A sequence of stops like TFTFTFTFTFTF (T = Stopping, F = Express) could be split as:
    Option 1: TFTF TFTF TFTF
    Option 2: TFTFTF TFTFTF

Decision Rule:
    The program defaults to sections of 3 stopping stations (T), whenever possible.
    If the total number of stopping stations isn’t divisible by 3, it will use as many 3-station sections as possible before switching to 2-station sections.

## **1. Installation & Setup**  

### **Prerequisites**  
- **.NET 8.0 SDK (or compatible runtime)**: [Download Here](https://dotnet.microsoft.com/en-us/download)
- **Visual Studio 2022** (or any compatible C# IDE)

### **Clone or Download the Repository**  
   git clone https://github.com/JaydenA123456789/QLDRail_TrainScheduler.git
