using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vitech.Genesys.Client;

namespace Pentateuch.Genesys
{
    /// <summary>
    /// Implements the IStructureItemVisitor interface to facilitate operating on each concrete structure item
    /// in a polymorphic manner.
    /// </summary>
    /// <seealso cref="Vitech.Genesys.Client.IStructureItemVisitor" />
    internal class StructureItemExporter : IStructureItemVisitor
    {
        int Indent { get; set; }

        public void VisitExit(IExitConstruct construct)
        {
            OutputIndent(construct);
            Console.Write("EXIT: " + construct.GetExit().Name);
            Console.WriteLine();
        }

        public void VisitExitBranch(IExitBranch branch)
        {
            OutputIndent(branch);
            Console.Write("EXIT BRANCH: " + branch.GetExit().Name);
            Console.WriteLine();
        }

        public void VisitFunction(IFunctionConstruct construct)
        {
            OutputIndent(construct);
            Console.Write("FUNCTION: " + construct.GetFunction().Name);
            Console.WriteLine();
        }

        public void VisitIterate(IIterateConstruct construct)
        {
            OutputIndent(construct);
            Console.Write(string.Format("ITERATE: Domain Set = {0}", construct.GetDomainSet().Name));
            Console.WriteLine();
        }

        public void VisitKillStatusBranch(IKillStatusBranch branch)
        {
            OutputIndent(branch);
            Console.Write(string.Format("{0} BRANCH: ", branch.Owner.ToString()));
            Console.WriteLine();
        }

        public void VisitLoop(ILoopConstruct construct)
        {
            OutputIndent(construct);
            Console.Write("LOOP");
            Console.WriteLine();
        }

        public void VisitLoopExit(ILoopExitConstruct construct)
        {
            OutputIndent(construct);
            Console.Write("LOOP EXIT");
            Console.WriteLine();
        }

        public void VisitParallel(IParallelConstruct construct)
        {
            OutputIndent(construct);
            Console.Write(string.Format("PARALLEL: Number of branches = {0}", construct.Branches.Count.ToString()));
            Console.WriteLine();
        }

        public void VisitReplicate(IReplicateConstruct construct)
        {
            OutputIndent(construct);
            Console.Write(string.Format("REPLICATE: Domain Set = {0}", construct.GetDomainSet().Name));
            Console.WriteLine();
        }

        public void VisitSelect(ISelectConstruct construct)
        {
            OutputIndent(construct);
            Console.Write(string.Format("SELECT: Number of branches = {0}", construct.Branches.Count.ToString()));
            Console.WriteLine();
        }

        public void VisitSelectionBranch(ISelectionBranch branch)
        {
            OutputIndent(branch);
            Console.Write(string.Format("{0} BRANCH: ", branch.Owner.ToString()));
            Console.WriteLine();
        }

        public void VisitStructureBranch(IStructureBranch branch)
        {
            OutputIndent(branch);
            Console.Write(string.Format("{0} BRANCH: ", (branch.Owner == null ? "MAIN" : branch.Owner.ToString())));
            Console.WriteLine();
        }

        /// <summary>
        /// Outputs spaces to the console as an indent based upon the number of owners the item has.
        /// </summary>
        /// <param name="item">The item.</param>
        private void OutputIndent(IStructureItem item)
        {
            int indent = NumberOfOwners(item);
            for (int i = 0; i < indent; i++)
                Console.Write("   ");
        }

        /// <summary>
        /// Returns the number of owners the item has.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        private int NumberOfOwners(IStructureItem item)
        {
            return item.Owner == null ? 0 : NumberOfOwners(item.Owner) + 1;
        }
    }
}
