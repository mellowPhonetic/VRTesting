using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using VelUtils;

namespace MellowVR
{
	public class MellowRig : Rig
	{
	    //public Rigidbody rb;
		//public Transform head;
		//public Transform leftHand;
		//public Transform rightHand;
	    public Transform hip;
	    public Transform leftFoot;
	    public Transform rightFoot;

		public Transform floor;
		public Transform playerTracking;
		public Transform playerCollider;
	
		/*
		public Transform GetHand(Side side)
		{
			switch (side)
			{
				case Side.Left:
					return leftHand;
				case Side.Right:
					return rightHand;
				case Side.Both:
				case Side.Either:
				case Side.None:
				default:
					throw new ArgumentOutOfRangeException(nameof(side), side, null);
			}
		}
		*/
	}
}