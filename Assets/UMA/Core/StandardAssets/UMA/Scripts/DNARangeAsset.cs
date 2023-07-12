﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine.Serialization;

namespace UMA
{
	/// <summary>
	/// Specifies the valid range of DNA values for a particular DNA converter and race.
	/// </summary>
	/// <remarks>
	/// Some DNA converters (for example the humanoid converters) don't actually produce
	/// valid results for the full range of DNA values. In this way the same converter
	/// can be used for multiple races, e.g. humans, elves, giants, and halflings can
	/// all be generated by the same HumanDNAConverterBehaviour. The DNA range asset
	/// is a way of specifying the values which are actually valid for a race.
	/// </remarks>
	[System.Serializable]
	public class DNARangeAsset : ScriptableObject 
	{

		[SerializeField]
		[Tooltip("The DNA converter for which the ranges apply. Accepts a DNAConverterController asset or a legacy DNAConverterBehaviour prefab.")]
		private DNAConverterField _dnaConverter = new DNAConverterField();

		/// <summary>
		/// The DNA converter for which the ranges apply.
		/// </summary>
		public IDNAConverter dnaConverter
		{
			get { return _dnaConverter.Value; }
			set { _dnaConverter.Value = value; }
		}

		public int EntryCount
		{
			get
			{
				if (_dnaConverter.Value != null)
				{
					if (dnaConverter.DNAType == typeof(DynamicUMADna))
					{
						return ((IDynamicDNAConverter)dnaConverter).dnaAsset.Names.Length;
					}
					else
					{
						var legacyDNA = dnaConverter.DNAType.GetConstructor(System.Type.EmptyTypes).Invoke(null) as UMADnaBase;
						if (legacyDNA != null)
						{
							return legacyDNA.Names.Length;
						}
					}
				}
				return 0;
			}
		}


		/// <summary>
		/// Finds any names in the given replacing converter, that match ones in the original converter
		/// </summary>
		/// <param name="originalConverter"></param>
		/// <param name="replacingConverter"></param>
		/// <returns>returns a dictionary of matching indexes, where the entry's index is the index in the replacing converter's dna and the entry's value is the corresponding index in the original converter's dna </returns>
		private Dictionary<int, int> GetMatchingIndexes(IDNAConverter originalConverter, IDNAConverter replacingConverter)
		{
			List<string> originalNames = new List<string>();
			List<string> replacingNames = new List<string>();
			UMADnaBase originalDNA;
			UMADnaBase replacingDNA;
			//original
			if (originalConverter.DNAType == typeof(DynamicUMADna))
			{
				originalNames.AddRange(((IDynamicDNAConverter)originalConverter).dnaAsset.Names);
			}
			else
			{
				originalDNA = originalConverter.DNAType.GetConstructor(System.Type.EmptyTypes).Invoke(null) as UMADnaBase;
				if (originalDNA != null)
				{
					originalNames.AddRange(originalDNA.Names);
				}
			}
			//replacing
			if (replacingConverter.DNAType == typeof(DynamicUMADna))
			{
				replacingNames.AddRange(((IDynamicDNAConverter)replacingConverter).dnaAsset.Names);
			}
			else
			{
				replacingDNA = replacingConverter.DNAType.GetConstructor(System.Type.EmptyTypes).Invoke(null) as UMADnaBase;
				if (replacingDNA != null)
				{
					replacingNames.AddRange(replacingDNA.Names);
				}
			}
			Dictionary<int, int> matchingIndexes = new Dictionary<int, int>();
			for (int i = 0; i < originalNames.Count; i++)
			{
				if (replacingNames.Contains(originalNames[i]))
					matchingIndexes.Add(i, replacingNames.IndexOf(originalNames[i]));
			}
			return matchingIndexes;
		}

		/// <summary>
		/// The mean (average) value for each DNA entry.
		/// </summary>
		public float[] means;
		/// <summary>
		/// The standard deviation for each DNA entry.
		/// </summary>
		/// <remarks>
		/// Used for Gaussian random values 99.7% of values will be within
		/// three standard deviations above or below the mean.
		/// </remarks>
		public float[] deviations;
		/// <summary>
		/// The spread above and below means for uniform ranges.
		/// </summary>
		public float[] spreads;

		private float[] values;

		/// <summary>
		/// Returns true if there is a dna range at the given index for the given name
		/// </summary>
		public bool ContainsDNARange(int index, string name)
		{
			if (dnaConverter == null)
				return false;

			//UMA 2.8 FixDNAPrefabs: why was this only working with DynamicUMADna?
			/*if (dnaConverter.DNAType == typeof(DynamicUMADna)) {
				if (((IDynamicDNAConverter)dnaConverter).dnaAsset.Names.Length > index) {
					if (Regex.Replace (((IDynamicDNAConverter)dnaConverter).dnaAsset.Names [index], "( )+", "") == Regex.Replace (name, "( )+", ""))
						return true;
				}
			}*/
			string[] names = new string[0];
			if (dnaConverter.DNAType == typeof(DynamicUMADna))
			{
				names = ((IDynamicDNAConverter)dnaConverter).dnaAsset.Names;
			}
			else
			{
				names = (dnaConverter.DNAType.GetConstructor(System.Type.EmptyTypes).Invoke(null) as UMADnaBase).Names;
			}
			//CharacterSystem.DNAEditor.Initialize calls this- who knew?
			if (index < names.Length && names[index] == name)
			{
				//Dont even bother with the Regex
				//if (Regex.Replace(names[index], "( )+", "") == Regex.Replace(name, "( )+", ""))
					return true;
			}

			return false;
		}

		/// <summary>
		/// Returns true if there is a dna range for the given name
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public bool ContainsDNARange(string name)
		{
			if (dnaConverter == null)
				return false;

			string[] names = new string[0];
			if (dnaConverter.DNAType == typeof(DynamicUMADna))
			{
				names = ((IDynamicDNAConverter)dnaConverter).dnaAsset.Names;
			}
			else
			{
				names = (dnaConverter.DNAType.GetConstructor(System.Type.EmptyTypes).Invoke(null) as UMADnaBase).Names;
			}

			for(int i = 0; i < names.Length; i++)
			{
				//Dont bother with the regex
				//if (Regex.Replace(names[i], "( )+", "") == Regex.Replace(name, "( )+", ""))
				if(names[i] == name)
					return true;
			}

			return false;
		}

		/// <summary>
		/// Returns the index of the dna range for the given name. Or -1 if there is no entry for the name
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public int IndexForDNAName(string name)
		{
			if (dnaConverter == null)
				return -1;

			string[] names = new string[0];
			if (dnaConverter.DNAType == typeof(DynamicUMADna))
			{
				names = ((IDynamicDNAConverter)dnaConverter).dnaAsset.Names;
			}
			else
			{
				names = (dnaConverter.DNAType.GetConstructor(System.Type.EmptyTypes).Invoke(null) as UMADnaBase).Names;
			}

			for (int i = 0; i < names.Length; i++)
			{
				//Dont bother with the regex
				//if (Regex.Replace(names[i], "( )+", "") == Regex.Replace(name, "( )+", ""))
				if(names[i] == name)
					return i;
			}

			return -1;
		}

		public bool ValueInRange(int index, float value)
		{
			float rangeMin = means[index] - spreads[index];
			float rangeMax = means[index] + spreads[index];
			if (value < rangeMin || value > rangeMax)
				return false;
			return true;
		}


		public Dictionary<string, DnaSetter> GetDNA(UMAData umaData, IDNAConverter dcb, string[] dbNames)
		{
			Dictionary<string, DnaSetter> dna = new Dictionary<string, DnaSetter>();

			foreach (UMADnaBase db in umaData.GetAllDna())
			{
				string Category = dcb.DisplayValue; 

				for (int i = 0; i < db.Count; i++)
				{
					if (dna.ContainsKey(dbNames[i]))
					{
						dna[db.Names[i]] = new DnaSetter(dbNames[i], db.Values[i], i, db, Category);
					}
					else
					{
						try
						{
							dna.Add(dbNames[i], new DnaSetter(dbNames[i], db.Values[i], i, db, Category));
						}
						catch(System.Exception ex)
						{
							Debug.LogException(ex);
						}
					}
				}
			}
			return dna;
		}

		private string[] dnaNames = { };

		/// <summary>
		/// Uniformly randomizes each value in the DNA.
		/// </summary>
		/// <param name="data">UMA data.</param>
		public void RandomizeDNA(UMAData data)
		{
			if (dnaConverter == null)
				return;

			UMADnaBase dna = data.GetDna(dnaConverter.DNATypeHash);
			if (dna == null)
				return;

			int entryCount = dna.Count;


			for(int i=0;i<means.Length;i++)
			{
				dna.SetValue(i, means[i] + (Random.value - 0.5f) * spreads[i]);
			}
		}

		/// <summary>
		/// Randomizes each value in the DNA using a Gaussian distribution.
		/// </summary>
		/// <param name="data">UMA data.</param>
		public void RandomizeDNAGaussian(UMAData data)
		{
			if (dnaConverter == null)
				return;

			UMADnaBase dna = data.GetDna(dnaConverter.DNATypeHash);
			if (dna == null)
				return;
			
			int entryCount = dna.Count;
			
			if (values == null)
				values = new float[entryCount];
			
			for (int i = 0; i < entryCount; i++)
			{
				if (i < means.Length)
					values[i] = UMAUtils.GaussianRandom(means[i], deviations[i]);
			}
			
			dna.Values = values;
		}
	}
}
