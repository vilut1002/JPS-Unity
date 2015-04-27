﻿using UnityEngine;
using System.Collections;

//[ExecuteInEditMode]
public class GridView : MonoBehaviour 
{
	public GameObject blockPrefab = null;

	[RangeAttribute(0, 255)]
	public int numBlocks = 1;
	public float blockSize = 0.64f;
	public int rowSize = 10;
	[RangeAttribute(0.0f, 1.0f)]
	public float blockBuffer = 0.0f;

	private int previousNumBlocks = 0;
	private float previousBuffer = 0;

	private GameObject[] childObjects = new GameObject[1];

	private Grid grid = new Grid();

	void resize()
	{
		// Kill all my children
		foreach ( GameObject child in childObjects )
		{
			DestroyImmediate( child );
		}

		// realloc the grids
		grid.gridNodes = new Node[numBlocks];
		childObjects   = new GameObject[numBlocks];

		for ( int i = 0; i < numBlocks ; ++i )
		{
			int row    = i % rowSize;
			int column = i / rowSize;
			
			// Create a new Child object
			GameObject child = Instantiate( blockPrefab );
			child.GetComponent<Transform>().parent = GetComponent<Transform>();  // Set as parent of this new child
			child.GetComponent<Transform>().localPosition = new Vector3(
				row    *  ( blockSize + blockBuffer ),
				column * -( blockSize + blockBuffer ),
				0.0f
			);

			grid.gridNodes[ i ] = new Node();
			grid.rowSize = this.rowSize;
			child.GetComponent<BlockScript>().nodeReference = grid.gridNodes[ i ]; // give the child a shared_ptr reference to the node it needs to act on

			childObjects[ i ] = child;
		}
	}

	// Update is called once per frame
	void Update () 
	{
		// If no one has given us a prefab to use, then don't make anything as we'll just get null pointer exception nonsense
		if ( blockPrefab == null )
			return;

		// If we need to resize then do
		if ( previousNumBlocks != numBlocks || previousBuffer != blockBuffer )
		{
			resize();
			previousNumBlocks = numBlocks;
			previousBuffer = blockBuffer;
		}
	}

	void OnGUI()
	{
		// Only Enabled this button, if We are in obstacle building mode
		GUI.enabled = JPSState.state == eJPSState.ST_OBSTACLE_BUILDING;

		if ( GUI.Button( new Rect( 10, 10, 200, 50 ), "Calculate Primary Jump Points") )
		{
            Debug.Log ( "You clicked the button!" );
            grid.buildPrimaryJumpPoints();    // Build primary Jump Points
            // Tell each child object to re-evaulte their rendering info
            foreach ( GameObject child in childObjects )
            {
            	BlockScript block_component = child.GetComponent<BlockScript>();
            	block_component.setSprite();	
            }
            JPSState.state = eJPSState.ST_PRIMARY_JPS_BUILDING; // transition state to Primary Jump Point Building State
        }
	}
}