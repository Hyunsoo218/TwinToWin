# Elemental Projectiles VFX Package, version 1.0.0
28/08/2023
© 2023-2033 - Pix Plays Studio

Asset is compatible with URP.
This package contains 4 Elemental Projectiles and their accompanying effects.

PREFABS
--------------------
    They are located in theirs respective folders under "PixPlays/ElementalProjectilesVFX/VFX/"

TESTING
--------------------
    -To test the package open the “DemoScene” located in “PixPlays/ElementalProjectilesVFX/Scenes/”

SCRIPTS
--------------------
Included is a script ProjectileTester which has settings for:
    -Projectile speed
    -Height curve (Used to manipulate the height of projectile during the flight)
    -Max Height (Multiplied by the sampled value from the curve)
    -Projectile delay (For some projectiles we may want to delay firing until cast effect finishes)
    -Delay Projectile Destroy (To let the trails disappear before projectile destruction)

USAGE
--------------------
    To use the effects you must combine the "Cast", "Projectile" and "Hit" Prefabs of the various spells. To help you achieve this you can use the script "ProjectileTester" or you can create you own custom script.
    IMPORTANT: All effects have been configured to self destruct using the ParticleSystems Destroy when finished setting.


HELP?
--------------------
For any suggestions, problems, or help contact us at:
support@pixplays.studio 

THANK YOU FOR DOWNLOADING, WE HOPE YOU ENJOY OUR PACKAGE!
PLEASE LEAVE A REVIEW OR RATE THE PACKAGE IF YOU FIND IT USEFUL!
We will be very greatfull.

RELEASE NOTES
-------------
1.0.0
-Initial release


