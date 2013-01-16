require 'albacore'
require 'fileutils'

@project = "Codestellation.DarkFlow"
@description = "Advanced threading and scheduling library."

@build_folder = Rake.application.original_dir + "/build"
@output = @build_folder + '/bin'

@nuget_dir = @build_folder + "/nuget"

task :default => [:all]


desc "Generate solution version "
assemblyinfo do |asm|
#Assuming we have tag '0.9-beta' git describe --abbrev=64 returns 0.9-beta-18-g408122de9c62e64937f5c1956a27cf4af9648c12
#If we have tag '0.9' git describe --abbrev=64 returns 0.9-18-g408122de9c62e64937f5c1956a27cf4af9648c12
#It should be parsed by  to generate correct assembly and nuget package version

	output = `git describe --abbrev=64`
	version_parts = output.match /(\d+).(\d+)-?([a-zA-Z]*)-(\d+)-(\w{7})/
	major = version_parts[1]
	minor = version_parts[2]
	revision = version_parts[4] || 0
	version_type = version_parts[3]
	hash = version_parts[5]
	
	@version = "#{major}.#{minor}.#{revision}"
	@package_version = @version
	@package_version += "-#{version_type}"  if (version_type || "").length > 0
	
	@product_version = @version
	@product_version += "-#{version_type}"  if (version_type || "").length > 0
    @product_version += "-#{hash}"
	
	asm.version = @version
	asm.file_version = @version
	asm.company_name = "Codestellation"
	asm.product_name = @project
	#asm.title = @project
	#asm.description = @description
	asm.copyright = "Copyright (c) 2012-2013 Codestellation"
	asm.custom_attributes :AssemblyInformationalVersionAttribute => @product_version
	asm.output_file = "SolutionVersion.cs"
	
end

desc "Build the application with msbuild"
msbuild :msbuild => [:assemblyinfo] do |msb|
	msb.properties :configuration => :Release, :OutputPath => @output
	msb.targets :Clean, :Build
	msb.solution = "#{@project}.sln"
	msb.verbosity = "quiet"
	msb.parameters = "/nologo"
end

desc "Run fixtures"
nunit :nunit => [:msbuild] do |nunit|
	nunit.command = "packages/NUnit.Runners.lite.2.6.2.20121104/nunit-console.exe" #Think how to automatically search for nunit-console
	nunit.assemblies "#{@output}/#{@project}.Tests.dll" #Automatically look up assemblies using mask *.Tests.dll
	nunit.options "/nologo"
end

desc "Packing DarkFlow project"
exec :nupack => [:nunit]do |nupack|
  nupack.command = ".nuget/nuget.exe"
  nupack.parameters "Pack DarkFlow/DarkFlow.csproj -Symbols -Build -Version #{@package_version}"
end


desc "Packing DarkFlow.Windsor project"
exec :nupack_windsor => [:nunit]do |nupack|
  nupack.command = ".nuget/nuget.exe"
  nupack.parameters "Pack DarkFlow.CastleWindsor/DarkFlow.CastleWindsor.csproj -Symbols -Build -Version #{@package_version}"
end

task :all => [:nupack, :nupack_windsor] do |task|
end