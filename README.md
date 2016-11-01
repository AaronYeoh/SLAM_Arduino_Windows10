# SLAM_Arduino_Windows10

A Universal Windows 10 app that maps the surroundings of a robot based on the sensor distance information sent over Serial.
Made for the MECHENG 706 Mechatronics Systems course - Simultaneous Localisation and Mapping Robot project.

### It's oddly similar to minesweeper.
Click squares to emulate a sensor reading at that position.
![Image of Simulation](https://i.gyazo.com/9e406f68968c57d4c676f2c30b50d2db.gif)

### Legend
* Black square = Robot has passed over that position
* Blue square = Robot detected obstacle
* White square = No obstacle at this position
* Grey square = Unknown state

### Simulation in action
![Image of Simulation3](https://i.gyazo.com/6ccafb6f55f7fc9e5fbc446abab3deb1.gif)
![Image of Simulation2](https://i.gyazo.com/826b2c9039a0d67b9e3e3f25fd286866.gif)


### Cool stuff
* Window's serial communications code was not production ready at the time I made this app. So I hacked a working one together.
* This whole app was hacked together over ~ 6 days. Just in time for the project demo.
