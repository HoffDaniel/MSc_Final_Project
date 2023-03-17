using System.Collections.Generic;
using UnityEngine;

public struct DNA
{
    //public List<Vector2> genes;
    public List<int> genes;

    //The available mutations
    public enum muta { RandomFlip, Swapp, Inversion, All }; // Add algorithm names here
    public enum CO {Percent25, Percent50, Percent75}; //to select
    public DNA(int nbr_genes)
    {

        genes = new List<int>();
        List<int> tmp_list = new();
        int temp_Count = nbr_genes;

        for (int i = 0; i < temp_Count; i++)
        {
            tmp_list.Add(i);
        }

        for (int i = 0; i < temp_Count; i++)
        {
            int tmp_rand = Random.Range(0, temp_Count);
            bool tmp_bool = genes.Contains(tmp_rand);

            while (tmp_bool)
            {
                tmp_list.Remove(tmp_rand);

                tmp_rand = tmp_list[Random.Range(0, tmp_list.Count)];

                tmp_bool = genes.Contains(tmp_rand);
            }
            //UnityEditor.EditorApplication.isPaused = true;
            genes.Add(tmp_rand);
        }
        //Add the return journey
        //Debug.Log(nbr_genes);
        genes.Add(genes[0]);
        
    }

    public DNA(DNA parent, DNA partner, muta mutationType = muta.All, CO crossover = CO.Percent25, float mutationRate = 0.01f, float crossoverRate = 0.25f)
    {
        genes = new List<int>();

        float co_value = -1;
        switch (crossover)
        {
            case CO.Percent25:
                co_value = 0.25f;
                break;
            case CO.Percent50:
                co_value = 0.50f;
                break;
            case CO.Percent75:
                co_value = 0.75f;
                break;
            default:
                break;
        }
        for (int i = 0; i < partner.genes.Count; i++)
        {
            genes.Add(partner.genes[i]);
        }


        //Debug.Log("TMP part genes count: " + genes.Count);

        float chance = Random.Range(0, 1);
        //float crossoverLength = 0.5f; // 50% parent 50% partner
        float temp_Length = parent.genes.Count * co_value;


        ////swapping a gene sequence with between partners => crossover
        if (chance < crossoverRate)
        {
            int index = parent.genes.Count - 1;

            genes.RemoveAt(index);//remove return gene

            int tCount = (int)temp_Length; //number of genes to be swapped with partner

            for (int i = 0; i < tCount; i++)
            {
                int temp_index = genes.FindIndex(a => a == parent.genes[i]);
                if (temp_index >= 0)
                {
                    int tmp_A = genes[temp_index];
                    int tmp_B = parent.genes[i];

                    //Swapp 
                    genes.Remove(tmp_A); //Removes the first 
                    genes.Add(tmp_B);
                    //genes[temp_index] = tmp_A;
                    //genes[i] = tmp_B; 

                }
            }
            genes.Add(genes[0]); //Re add the return
        }
        float mutationChance = Random.Range(0.0f, 10.0f);
        if (mutationChance <= mutationRate)
        {
            int maxIndex = parent.genes.Count - 1;
            int tmp_nbr_swapps = Random.Range(0, (int)((maxIndex) * 0.5));
            switch (mutationType)
            {
                case muta.RandomFlip:
                    Mutation_Swapp(1, maxIndex, true);
                    break;
                case muta.Swapp:
                    Mutation_Swapp(tmp_nbr_swapps, maxIndex);
                    break;
                case muta.Inversion:
                    Mutation_Inversion(tmp_nbr_swapps);
                    break;
                case muta.All: //Change so its no alll all at the same time but but alternating
                    Mutation_Swapp(1, maxIndex, true);
                    Mutation_Swapp(tmp_nbr_swapps, maxIndex);
                    Mutation_Inversion(tmp_nbr_swapps);
                    break;
                default:
                    break;
            }
        }
    }

    public DNA(DNA parent, DNA partner, float mutationRate = 0.01f, float crossoverRate = 0.25f, float crossoverLength = 0.25f)
    {        
        genes = new List<int>();

        for (int i = 0; i < partner.genes.Count; i++)
        {
            genes.Add(partner.genes[i]);
        }
        

        //Debug.Log("TMP part genes count: " + genes.Count);

        float chance = Random.Range(0, 1);
        //float crossoverLength = 0.5f; // 50% parent 50% partner
        float temp_Length = parent.genes.Count * crossoverLength;


        ////swapping a gene sequence with between partners => crossover
        if (chance < crossoverRate)
        {
            int index = parent.genes.Count - 1;

            genes.RemoveAt(index);//remove return gene

            int tCount = (int)temp_Length; //number of genes to be swapped with partner

            for (int i = 0; i < tCount; i++)
            {
                int temp_index = genes.FindIndex(a => a == parent.genes[i]);
                if (temp_index >= 0)
                {
                    int tmp_A = genes[temp_index];
                    int tmp_B = parent.genes[i];

                    //Swapp 
                    genes.Remove(tmp_A); //Removes the first 
                    genes.Add(tmp_B);
                    //genes[temp_index] = tmp_A;
                    //genes[i] = tmp_B; 

                }
            }
            genes.Add(genes[0]); //Re add the return
        }
        
        bool isRandom = false;
        bool isInversion = true;        
        bool isTwist = false;

        //MUTATION Random
        
        float mutationChance = Random.Range(0.0f, 10.0f);
        if (mutationChance <= mutationRate)
        {
            int maxIndex = parent.genes.Count - 1;
            int tmp_nbr_swapps = Random.Range(0, (int)((maxIndex) * 0.5));

            if (isRandom)
            {                
                Mutation_Swapp(1, maxIndex, true);
            }

            if (isInversion)
            {
                Mutation_Inversion(tmp_nbr_swapps); 
            }
                       
            if (isTwist)
            {
                Mutation_Swapp(tmp_nbr_swapps, maxIndex);
                return;
            }

        }

        ////MUTATION Random
        //int maxIndex = parent.genes.Count - 1;
        //float mutationChance = Random.Range(0.0f, 10.0f);
        //if (mutationChance <= mutationRate)
        //{
        //   // Debug.Log("RANDOM MUTATION: ");
        //    int Rand = Random.Range(0, 10);// This Could be used for a randome number of twists
        //    twistMutation(1, maxIndex, true); 
        //    return;
        //}
        //int tmp_nbr_swapps = Random.Range(0, (int)((maxIndex) * 0.5));

        //float mutationChance2 = Random.Range(0.0f, 10.0f);
        //if (mutationChance2 <= mutationRate)
        //{
        //    Mutation_Inversion(tmp_nbr_swapps); // Could be done for a randome number of twists
        //    return;
        //}

        
        //float TwistMutationChance = Random.Range(0.0f, 10f);
        //if (TwistMutationChance <= mutationRate)
        //{
        //    twistMutation(tmp_nbr_swapps, maxIndex);
        //    return;
        //}


    }

    //Mutations:
    // - Swap Mutation      / flip mutation -> research: are they the same?
    // - Scramble Mutation
    // - Inversion Mutation
    public void Mutation_Inversion(int nbr_of_swaps)
    {
        int maxIndex = genes.Count - 1;
        // 0 is min Index
        genes.RemoveAt(maxIndex);//Remove return value //this could be simplified so that we only add the return gene in the calculations
        maxIndex--;

        //Debug.Log("New Twist MUTATION | " + nbr_of_swaps);
        int twistAtGene = Random.Range(0, maxIndex);

        int swapGene1 = twistAtGene + 1;
        int swapGene2 = twistAtGene - 1;

        for (int i = 0; i < nbr_of_swaps; i++)
        {
            int fixedIndex1 = FixIndex(swapGene1);
            int fixedIndex2 = FixIndex(swapGene2);
            int tmp_gene = genes[fixedIndex1];
            
            genes[fixedIndex1] = genes[fixedIndex2];
            genes[fixedIndex2] = tmp_gene;
            swapGene1++;
            swapGene2--;

        }

        genes.Add(genes[0]); //Add return

    }
    public void Mutation_Swapp(int nbr_of_swaps, int maxIndex, bool isRand = false)
    {
        //Debug.Log("Twist MUTATION | " + nbr_of_swaps);
        int tmp_max = maxIndex - nbr_of_swaps * 2;
        if (tmp_max < 0) return;
        

        int mutatedGene = Random.Range(nbr_of_swaps, tmp_max);
        int swapGene = mutatedGene + 1;

        for (int i = 1; i <= nbr_of_swaps; i++)
        {
            if (isRand)
            {
                swapGene = Random.Range(nbr_of_swaps, tmp_max);
                mutatedGene = Random.Range(nbr_of_swaps, tmp_max);
            }

            var tmp_g = genes[mutatedGene];
            genes[mutatedGene] = genes[swapGene];
            genes[swapGene] = tmp_g;

            mutatedGene += i * 2;
            swapGene -= i * 2;

            if (mutatedGene > maxIndex || swapGene < 0) return;
        }

    }

    public void AddReturn()
    {
        genes.Add(genes[0]);
    }

    public void RemoveReturn()
    {
        genes.RemoveAt(genes.Count - 1);
    }

    public int FixIndex(int index)
    {
        int maxIndex = genes.Count - 1;
        int tmp_index;
        if (index > maxIndex)
        {
            tmp_index = index - maxIndex;
        }
        else if (index < 0)
        {
            tmp_index = index + maxIndex;
        }
        else
        {
            tmp_index = index;
        }
        return tmp_index;
    }

}

