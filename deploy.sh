VERSION=$(cat version)

if [ -z $TRAVIS_TAG ]
then
	printf -v build %05d $TRAVIS_BUILD_NUMBER
	VERSION=$VERSION-$build
fi

dotnet pack "src/Discord.Addons.EventQueue.csproj" -c "Release" -o "./artifacts/" /p:Version=$VERSION

dotnet nuget push "artifacts/*" -s $MYGET_SOURCE -k $MYGET_KEY