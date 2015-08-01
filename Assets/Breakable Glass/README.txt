1. Creating a glass.
 To create a new glass just drag "Glass" object to your scene.


2. Scaling and rotating.
 You can scale and rotate the Glass GameObject as you want but note that rigidbodies with thin colliders have some problems with collision.


3. Setting up BreakGlass Component.
  1) BrokenGlassGO list contains possible variations of broken glass (shards). 

  2) BreakSound determines if sound is played when the glass breaks.

  3) SoundEmitter determines a GameObject that will be spawned when the glass breaks.

  4) SoundEmitterLifetime is time in seconds that SoundEmitter will exist.

  5) ShardsLifetime is time in seconds that shards of broken glass will exist. Set this to 0 if you don't want the shards to be removed.

  6) ShardMass is rigidbody mass of each shard.

  7) ShardMaterial is a material that will be applied to the shards.

  8) BreakByVelocity determines if glass should be broken when a rigidbody hits it at certain velocity.

  9) BreakVelocity is that "certain velocity".

  10) BreakByImpulse determines if glass should be broken when a rigidbody with specific impulse hits it. Impulse = rigidbody.mass * relativeVelocity

  11) BreakByClick determines if glass should be broken by a click.

  12) SlowdownCoefficient is a percent of velocity that object that hit the glass will have after breaking the glass.
      For example a ball had a velocity of 10 before the hit and right after the hit his velocity changed to 6. This means that SlowdownCoefficient = 0.6 .

4. Changing the sound.
  To change the sound you just need to edit the "SoundEmitter" GameObject, or create a new one. 
  SoundEmitter is an empty GameObject that has AudioSource component with the sound of breaking glass attached to it. Note that in AudioSource Play On Awake has
  to be checked.

5. To make your rigidbody able to break glass you need to apply GlassBreaker script to it.