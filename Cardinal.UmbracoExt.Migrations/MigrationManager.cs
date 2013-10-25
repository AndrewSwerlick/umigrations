using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core;
using Umbraco.Core.Persistence;
using Umbraco.Core.Services;

namespace Cardinal.UmbracoExt.Migrations
{
    public class MigrationManager
    {
        private readonly MigrationContext _context;
        private readonly List<VersionNumber> _versionsRegistered;
        private readonly ApplicationContext _appContext;
        private readonly IDictionary<VersionNumber, IMigrationScript> _scripts;
        public IDictionary<VersionNumber, IMigrationScript> Scripts
        {
            get
            {
                return _scripts;
            }
        }

        public MigrationManager(MigrationContext context)
        {
            _context = context;
            _appContext = context.AppContext;
            _versionsRegistered = new List<VersionNumber>();
            _scripts = new Dictionary<VersionNumber, IMigrationScript>();

        }        

        public void RunScript(IMigrationScript script)
        {
            script.Execute(_appContext);
        }

        public void RegisterScripts(string scriptsNamespace, Assembly assembly)
        {
            var scriptTypes = assembly.GetTypes().Where(t => t.Namespace == scriptsNamespace && typeof(IMigrationScript).IsAssignableFrom(t) && t.IsClass && !t.IsAbstract);
            RegisterScripts(scriptTypes.Select(t => Activator.CreateInstance(t) as IMigrationScript).ToList());
        }

        public void RegisterScripts(IList<IMigrationScript> scripts)
        {
            var allScriptsHaveVersions = scripts.All(s => TypeDescriptor.GetAttributes(s.GetType()).Cast<Attribute>().Any(a => a.GetType() == typeof(VersionNumberAttribute)));
            if (!allScriptsHaveVersions)
                throw new ArgumentException("All upgrade scripts must have a VersionNumber Attribute on the class specifying what version this script is for. Please add the version attribute to your class");

            var scriptsWithNumbers = scripts.Select(s =>
            {
                var attribute = TypeDescriptor.GetAttributes(s.GetType()).Cast<Attribute>().Single(a => a.GetType() == typeof(VersionNumberAttribute));
                var versionNumber = ((VersionNumberAttribute)attribute).VersionProperty;
                return new KeyValuePair<VersionNumber, IMigrationScript>(versionNumber,s);
            }).ToList();

            var versionsToRegister = scriptsWithNumbers.Select(v => v.Key).ToList();

            var versionsAlreadyRegistered = versionsToRegister.Intersect(_versionsRegistered).ToList();
            var someOfTheseVersionsAreAlreadyRegistered = versionsAlreadyRegistered.Any();
            if (someOfTheseVersionsAreAlreadyRegistered)
                throw new ArgumentException(string.Format("The versions {0} have already been registered with the deployment manager", string.Join(",", versionsAlreadyRegistered.Select(v => v.ToString()).ToArray())));

            var duplicateVersionNumbersInProvidedList = versionsToRegister.GroupBy(v => v.GetHashCode()).Any(g => g.Count() > 1);
            if (duplicateVersionNumbersInProvidedList)
                throw new ArgumentException("The provided list of scripts to register contains duplicate version numbers");
            foreach (var migrationScript in scriptsWithNumbers)
            {
                _scripts.Add(migrationScript);
            }
            _versionsRegistered.AddRange(versionsToRegister);
        }

        public void Migrate(VersionNumber from, VersionNumber to)
        {
            var scriptsToRun = _scripts.Where(s =>
            {               
                var versionNumber =s.Key;
                return versionNumber > from && (versionNumber < to || versionNumber == to);
            });

            var orderedScripts = scriptsToRun.OrderBy(s =>
                {
                    var versionNumber = s.Key;
                    return versionNumber;
                });
            
            foreach (KeyValuePair<VersionNumber,IMigrationScript> script in orderedScripts)
            {
                script.Value.Execute(_appContext);
            }

            _context.AppContext.DatabaseContext.Database.Insert(new Migration { VersionString = to.ToString() });
        }

        public void Migrate()
        {
            if(string.IsNullOrEmpty(_context.Settings.ScriptsNameSpace) && _scripts.Count == 0)
                throw new InvalidOperationException("The migration settings do not define a script namespace for migration scripts. Please editing the settings object, or register scripts using the RegisterScripts method");

            if(_scripts.Count == 0)
                RegisterScripts(_context.Settings.ScriptsNameSpace, Assembly.Load(_context.Settings.Assembly));

            if (_context.To == null)
                _context.To = _scripts.OrderByDescending(v => v.Key).First().Key;

            if(_context.From == null || _context.To == null)
                throw new InvalidOperationException("Cannot determine what version to migration to or from");

            if (_context.Settings.ReRunLastScript)
            {
                if (_scripts.Count > 1)
                {
                    var scriptBeforeLastNumber =
                        _scripts.Where(v => v.Key < _context.To).OrderByDescending(v => v.Key).First().Key;
                    _context.From = scriptBeforeLastNumber < _context.From ? scriptBeforeLastNumber : _context.From;
                }
                else
                {
                    _context.From = new VersionNumber("0.0");
                }
            }

            Migrate(_context.From,_context.To);
        }
    }
}
