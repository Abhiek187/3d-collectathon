using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace Tests
{
    public class TestSuite
    {
        [UnitySetUp]
        public IEnumerator Setup()
        {
            // Load the Game Scene and wait for everything to load
            SceneManager.LoadScene("GameScene");
            yield return null;
        }

        [UnityTest]
        public IEnumerator MiniGemSpins()
        {
            // The Mini Gems should always be rotating
            // Given
            GameObject miniGem = GameObject.Find("Mini Gem");
            Quaternion oldRotation = miniGem.transform.rotation;
            // When
            yield return null; // wait for a frame
            // Then
            Quaternion newRotation = miniGem.transform.rotation;
            Assert.AreNotEqual(newRotation, oldRotation);
        }

        [UnityTest]
        public IEnumerator MegaGemSpins()
        {
            // The Mega Gems should always be rotating
            // Given
            GameObject megaGem = GameObject.Find("Mega Gem");
            Quaternion oldRotation = megaGem.transform.rotation;
            // When
            yield return null; // wait for a frame
            // Then
            Quaternion newRotation = megaGem.transform.rotation;
            Assert.AreNotEqual(newRotation, oldRotation);
        }

        public IEnumerator FloatingPlatformMoves()
        {
            // The floating platforms should always be moving
            GameObject floatingPlatform = GameObject.Find("Floating Platform");
            Vector3 oldPosition = floatingPlatform.transform.position;
            yield return null;

            Vector3 newPosition = floatingPlatform.transform.position;
            Assert.AreNotEqual(newPosition, oldPosition);
        }

        [UnityTest]
        public IEnumerator GameEndsAfterCollectingAllMegaGems()
        {
            // At the end of the game, it should be night time and the fireworks should appear
            GameObject megaGems = GameObject.Find("Mega Gems");
            GameObject sun = GameObject.Find("Sunlight");
            Material oldSkybox = RenderSettings.skybox;

            // Collect (remove) all the mega gems to trigger the ending
            foreach (Transform megaGem in megaGems.transform)
            {
                Object.Destroy(megaGem.gameObject);
            }

            yield return new WaitForSeconds(5); // wait for the sun's intensity to reach 0

            // Check that there aren't any more Mega Gems
            Assert.AreEqual(megaGems.transform.childCount, 0);
            // Check that the sun is gone
            UnityEngine.Assertions.Assert.IsNull(sun);
            // Check for the night skybox
            Material newSkybox = RenderSettings.skybox;
            Assert.AreNotEqual(newSkybox, oldSkybox);
            // Check that the fireworks are playing
            ParticleSystem fireworks = GameObject.Find("Fireworks Rocket").GetComponent<ParticleSystem>();
            Assert.True(fireworks.isPlaying);
            // Check that the congratulation message and the restart button are present
            GameObject congrats = GameObject.Find("Congrats");
            GameObject restartButton = GameObject.Find("Restart Button");
            Assert.True(congrats.activeInHierarchy);
            Assert.True(restartButton.activeInHierarchy);
        }

        [UnityTest]
        public IEnumerator SnowballGrowsAsItRollsInSnow()
        {
            // Check that the snowball increases in size as it's rolled around in the snow
            GameObject snowball = GameObject.Find("Snowball");
            Rigidbody snowballRigidbody = snowball.GetComponent<Rigidbody>();
            Vector3 oldPosition = snowball.transform.position;
            float oldSize = snowball.transform.localScale.magnitude;

            // Move the snowball toward the snow
            snowballRigidbody.velocity = Vector3.left * 10;
            yield return new WaitForSeconds(1);

            Vector3 newPosition = snowball.transform.position;
            float newSize = snowball.transform.localScale.magnitude;
            Assert.Less(newPosition.x, oldPosition.x);
            Assert.Greater(newSize, oldSize);
        }

        [UnityTest]
        public IEnumerator SnowballMeltsAsItRollsOutsideSnow()
        {
            // Check that the snowball decreases in size as it's rolled around outside the snow area
            GameObject snowball = GameObject.Find("Snowball");
            Rigidbody snowballRigidbody = snowball.GetComponent<Rigidbody>();
            Vector3 oldPosition = snowball.transform.position;
            float oldSize = snowball.transform.localScale.magnitude;

            // Move the snowball away from the snow
            snowballRigidbody.velocity = Vector3.back * 20;
            yield return new WaitForSeconds(2);

            Vector3 newPosition = snowball.transform.position;
            float newSize = snowball.transform.localScale.magnitude;
            Assert.Less(newPosition.z, oldPosition.z);
            Assert.Less(newSize, oldSize);
        }

        [UnityTest]
        public IEnumerator SnowballRespawnsIfItFalls()
        {
            // Check that the snowball returns to its original position if it falls off the cliff
            GameObject snowball = GameObject.Find("Snowball");
            Rigidbody snowballRigidbody = snowball.GetComponent<Rigidbody>();
            Vector3 oldPosition = snowball.transform.position;

            // Move the snowball toward the cliff
            snowballRigidbody.velocity = Vector3.right * 30;
            yield return new WaitForSeconds(2);

            Vector3 newPosition = snowball.transform.position;
            // The snowball could be bouncing, so check the other two dimensions
            Assert.AreEqual(newPosition.x, oldPosition.x);
            Assert.AreEqual(newPosition.z, oldPosition.z);
        }

        [UnityTest]
        public IEnumerator SnowballPressesButton()
        {
            // Check that the buttons get pressed after rolling a snowball onto them
            GameObject snowball = GameObject.Find("Snowball");
            GameObject button = GameObject.Find("Button");
            Vector3 oldButtonPosition = button.transform.position;
            Material oldMaterial = button.GetComponent<Renderer>().material;

            // Land the snowball on top of the button
            snowball.transform.position = button.transform.position + Vector3.up * 10;
            // Wait for gravity and the button coroutine to finish
            yield return new WaitForSeconds(2);

            // Check that the button is lowered and green
            Vector3 newButtonPosition = button.transform.position;
            Material newMaterial = button.GetComponent<Renderer>().material;
            Assert.Less(newButtonPosition.y, oldButtonPosition.y);
            Assert.AreNotEqual(newMaterial, oldMaterial);
        }

        [UnityTest]
        public IEnumerator HiddenPlatformRisesAfterAllButtonsArePressed()
        {
            // Check that the hidden platform appears after all 3 buttons are pressed
            GameObject hiddenPlatform = GameObject.Find("Hidden Platform");
            Vector3 oldPlatformPosition = hiddenPlatform.transform.position;

            // Put the 3 snowballs on each button
            GameObject snowball = GameObject.Find("Snowball");
            GameObject button = GameObject.Find("Button");
            snowball.transform.position = button.transform.position + Vector3.up * 10;
            GameObject snowball2 = GameObject.Find("Snowball (1)");
            GameObject button2 = GameObject.Find("Button (1)");
            snowball2.transform.position = button2.transform.position + Vector3.up * 10;
            GameObject snowball3 = GameObject.Find("Snowball (2)");
            GameObject button3 = GameObject.Find("Button (2)");
            snowball3.transform.position = button3.transform.position + Vector3.up * 10;
            
            // Wait for all the buttons to be pressed and the hidden platform to rise
            yield return new WaitForSeconds(3);

            Vector3 newPlatformPosition = hiddenPlatform.transform.position;
            Assert.Greater(newPlatformPosition.y, oldPlatformPosition.y);
        }

        [UnityTest]
        public IEnumerator FireballRotates()
        {
            // The fireballs should always be moving
            GameObject fireball = GameObject.Find("Fireball");
            Vector3 oldPosition = fireball.transform.position;
            yield return null;

            Vector3 newPosition = fireball.transform.position;
            Assert.AreNotEqual(newPosition, oldPosition);
        }

        [UnityTest]
        public IEnumerator MagicCarpetIsAtRest()
        {
            // The magic carpet shouldn't move if the player isn't on it
            GameObject magicCarpet = GameObject.Find("Magic Carpet");
            Vector3 oldPosition = magicCarpet.transform.position;
            yield return null;

            Vector3 newPosition = magicCarpet.transform.position;
            Assert.AreEqual(newPosition, oldPosition);
        }
    }
}
